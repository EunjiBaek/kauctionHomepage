(function ($) {
    'use strict';

    $.auction = {
        connection: null,
        bidListSize: 100,
        bidListTarget: null,
        bidMyListTarget: null,
        bidEmptyTarget: null,
        bidEmptyMyTarget: null,
        aucKind: null,
        aucNum: null,
        workUid: null,
        wishYN: "N",
        priceStart: null,
        priceMax: null,
        priceAscend: null,
        priceNext: null,
        myHighestPrice: 0,
        bidEndNoti: false,
        reqProcess: false,
        connectionID: null,

        /*------------------------------------------------------------------
        * @function:    init
        * @parameter:   config (설정값 json)
        * @description: 경매 상태값 초기화
        ------------------------------------------------------------------*/
        init: function (config) {
            try {
                var auction = $.auction;
                auction.aucKind = config["auc_kind"];
                auction.aucNum = config["auc_num"];
                auction.workUid = config["work_uid"];
                auction.bidListTarget = config["bid_list"];
                auction.bidMyListTarget = config["bid_my_list"];
                auction.bidEmptyTarget = config["bid_empty"];
                auction.bidEmptyMyTarget = config["bid_my_empty"];
                auction.wishYN = config["wish_yn"];

                auction.priceStart = config["price_start"];
                auction.priceMax = config["price_max"];
                auction.priceAscend = config["price_ascend"];

                if (auction.priceMax === "0" || auction.priceMax === "-") {
                    auction.priceStart = parseInt(auction.priceStart.toString().replace(/,/ig, ""), 10);
                } else {
                    auction.priceStart = $.stringUtils.comma(parseInt(auction.priceMax.toString().replace(/,/ig, ""), 10) + parseInt(auction.priceAscend.toString().replace(/,/ig, ""), 10));
                }

                Object.defineProperty(WebSocket, 'OPEN', { value: 1, });

                auction.connection = new signalR.HubConnectionBuilder().withUrl("/auctionHub").withAutomaticReconnect().build();

                auction.connection.start().then(function () {
                    console.log('connection - init');
                    try {
                        auction.connection.invoke("AddToGroup", "group-" + auction.aucKind + "-" + auction.aucNum + "-" + auction.workUid);
                        console.log('connection - addToGroup');
                    }
                    catch (e) {
                        console.error(e.toString());
                    }
                }).catch(function (err) {
                    return console.error(err.toString());
                });

                auction.connection.on("Send", function (connectionID) {
                    $.auction.connectionID = connectionID;
                    console.log($.auction.connectionID);
                });

                auction.connection.on("UpdateInfo", function (state, id) {
                    if (state === "refresh") {
                        $.auction.getBidTotalList('refresh');
                    }
                });

            } catch (e) { console.log(e.description); }
        },

        sendBidMessage: function () {
            var auction = $.auction;
            if (auction.connection !== null && auction.connectionID !== null) {
                auction.connection.invoke("SendMessageToGroup", "group-" + auction.aucKind + "-" + auction.aucNum + "-" + auction.workUid, $.auction.connectionID)
                    .catch(function (err) {
                        console.error('sendBidMessage - ' + err.toString());
                    });
            }
        },

        removeFromGroup: function () {
            var auction = $.auction;
            if (auction.connection !== null) {
                auction.connection.invoke("RemoveFromGroup", "group-" + auction.aucKind + "-" + auction.aucNum + "-" + auction.workUid)
                    .catch(function (err) {
                        console.error('removeFromGroup - ' + err.toString());
                    });
            }
        },

        getBidList: function (mode) {
            var param = {};
            param["mode"] = typeof mode === "string" ? mode : "";
            $.ajaxUtils.getApiData("/api/Auction/BidList/" + $.auction.workUid.toString(), param, $.auction.getBidListComplete);
        },

        getBidListComplete: function (result, parameters) {
            if ($.ajaxUtils.getResultCode(result) === "00") {
                $($.auction.bidListTarget).empty();
                $($.auction.bidMyListTarget).empty();
                $($.auction.bidListTarget).show();
                var count = -1;
                var totalCount = 0;
                var myCount = 0;
                var myFlag = false;
                $.each(result.data, function (index, item) {
                    totalCount = item.total_count;
                    myCount = item.my_count;

                    if (index < $.auction.bidListSize) {
                        var li = $('<li />');

                        // li > span
                        var bidDate = $('<span />', { "class": "bid-date" }).append(item.reg_ymd);

                        // li > div > div.bid-left
                        var bidContLeft = $('<div />', { "class": "bid-left" }).append(
                            $('<span />', { "class": "time" }).append(item.reg_hms),
                            $('<span />', { "class": "id" }).append(item.mem_id)
                        );

                        // li > div > div.bid-right
                        var bidContRight = $('<div />', { "class": "bid-right" });

                        // 2021.10.06 :: 최고가 표시 수정
                        // if (index === 0) {
                        if (item.bid_uid === item.highest_uid) {
                            bidContRight.append(ka.icon.highest);
                        }
                        if (item.my_bid === "Y") {
                            bidContRight.append($.auction.getMyBidTag);
                            myFlag = true;
                        }
                        bidContRight.append($('<span />', { "class": "price" }).append($.stringUtils.comma(item.price_bid)));

                        // li > div
                        var bidCont = $('<div />').append(bidContLeft, bidContRight);

                        li.append(bidDate, bidCont);
                        $($.auction.bidListTarget).append(li);

                        if (myFlag) {
                            $('.bidlist-desc').find('.b2').find('p').hide();
                            $($.auction.bidMyListTarget).append(li);
                        }
                        myFlag = false;

                        // 현재가 업데이트 처리
                        if (index === 0) {
                            $('.price-max').html($.stringUtils.comma(item.price_bid));
                        }
                    }
                    count = index + 1;
                });
                if (count < 1) {
                    $($.auction.bidEmptyTarget).css("opacity", "100");
                    $($.auction.bidEmptyTarget).show();
                } else if (count >= 4) {
                    $(".bid-price .bid-list .bidlist-desc > div.b1").addClass("scroll");
                    $(".bidlistmb-desc > div.b1").addClass("scroll");
                } else {
                    $(".bid-price .bid-list .bidlist-desc > div.b1").removeClass("scroll");
                    $(".bidlistmb-desc > div.b1").removeClass("scroll");
                }

                if (myCount < 1) {
                    $($.auction.bidEmptyMyTarget).css("opacity", "100");
                } else if (myCount > 4) {
                    $(".bid-price .bid-list .bidlist-desc > div.b2").addClass("scroll");
                    $(".bidlistmb-desc > div.b2").addClass("scroll");
                } else {
                    $(".bid-price .bid-list .bidlist-desc > div.b2").removeClass("scroll");
                    $(".bidlistmb-desc > div.b2").removeClass("scroll");
                }

                $($.auction.bidListTarget + "-my").html("(" + myCount.toString() + ")");
                $($.auction.bidListTarget + "-total").html("(" + totalCount.toString() + ")");
                $($.auction.bidListTarget + "-total2").html("(" + ka.msg.auction.bidCount + ' <span class="num n2">' + totalCount.toString() + "</span>" + ka.msg.auction.bidCount2 + ")");
                $($.auction.bidListTarget + "-mb-total2").html("(" + ka.msg.auction.bidCount + ' <span class="num n2">' + totalCount.toString() + "</span>" + ka.msg.auction.bidCount2 + ")");
                $($.auction.bidListTarget + "-total3").html("(" + totalCount + ka.msg.auction.bidCount2 + ")");

                try {
                    if (parameters !== null && JSON.parse(parameters).mode === "refresh") {
                        $.commonUtils.notify(ka.msg.auction.refresh);
                    }
                } catch (e) { }
            }
        },

        getBidTotalList: function (mode) {
            if ($.auction.reqProcess) return false;
            $.auction.reqProcess = true;
            var param = {};
            param["mode"] = typeof mode === "string" ? mode : "";
            $.ajaxUtils.getApiData("/api/Auction/BidTotalList/" + $.auction.workUid.toString(), param, $.auction.getBidTotalListComplete);
        },

        getBidTotalListComplete: function (result, parameters) {
            $.auction.reqProcess = false;

            if ($.ajaxUtils.getResultCode(result) === "00") {
                if (result.data == null) return false;

                $($.auction.bidListTarget).empty();
                $($.auction.bidMyListTarget).empty();
                $($.auction.bidListTarget).show();
                var count = -1;
                var totalCount = 0;
                var myCount = 0;
                var myFlag = false;

                // BidList
                $.each(result.data.bid_list, function (index, item) {

                    totalCount = item.total_count;
                    myCount = item.my_count;

                    if (index < $.auction.bidListSize) {
                        var li = $('<li />');

                        // li > span
                        var bidDate = $('<span />', { "class": "bid-date" }).append(item.reg_ymd);

                        // li > div > div.bid-left
                        var bidContLeft = $('<div />', { "class": "bid-left" }).append(
                            $('<span />', { "class": "time" }).append(item.reg_hms),
                            $('<span />', { "class": "id" }).append(item.mem_id)
                        );
                        
                        // li > div > div.bid-right
                        var bidContRight = $('<div />', { "class": "bid-right" });

                        // 2021.10.06 :: 최고가 표시 수정
                        // if (index === 0) {
                        if (item.bid_uid === item.highest_uid) {
                            bidContRight.append(ka.icon.highest);
                        }
                        if (item.my_bid === "Y") {
                            bidContRight.append($.auction.getMyBidTag);
                            myFlag = true;
                        }
                        bidContRight.append($('<span />', { "class": "price" }).append($.stringUtils.comma(item.price_bid)));

                        // li > div
                        var bidCont = $('<div />').append(bidContLeft, bidContRight);

                        li.append(bidDate, bidCont);
                        $($.auction.bidListTarget).append(li);

                        if (myFlag) {
                            $('.bidlist-desc').find('.b2').find('p').hide();
                            var cloneEl = li.clone();
                            $($.auction.bidMyListTarget).append(cloneEl);
                        }
                        myFlag = false;

                        // 현재가 업데이트 처리
                        if (index === 0) {
                            $('.price-max').html($.stringUtils.comma(item.price_bid));
                            $.auction.priceStart = $.stringUtils.comma(item.price_bid);
                        }
                    }
                    count = index + 1;
                });
                if (count < 1) {
                    $($.auction.bidEmptyTarget).css("opacity", "100");
                    $($.auction.bidEmptyTarget).show();
                } else if (count >= 4) {
                    $(".bid-price .bid-list .bidlist-desc > div.b1").addClass("scroll");
                    $(".bidlistmb-desc > div.b1").addClass("scroll");
                } else {
                    $(".bid-price .bid-list .bidlist-desc > div.b1").removeClass("scroll");
                    $(".bidlistmb-desc > div.b1").removeClass("scroll");
                }

                if (myCount < 1) {
                    $($.auction.bidEmptyMyTarget).css("opacity", "100");
                    $($.auction.bidEmptyMyTarget).show();
                } else if (myCount > 4) {
                    $(".bid-price .bid-list .bidlist-desc > div.b2").addClass("scroll");
                    $(".bidlistmb-desc > div.b2").addClass("scroll");
                } else {
                    $(".bid-price .bid-list .bidlist-desc > div.b2").removeClass("scroll");
                    $(".bidlistmb-desc > div.b2").removeClass("scroll");
                }

                // BidPrice
                var target = $('.bid-price-list');
                if (target.length > 0) {
                    target.empty();
                    $.each(result.data.bid_price, function (index, item) {
                        var el = $('<li />', {
                            "data-index": item.Index,
                            "data-price": item.PriceBid,
                            "class": "price-item price-" + item.PriceBid
                        }).append("KRW " + $.stringUtils.comma(item.PriceBid));
                        el.on("click", function (e) {
                            e.preventDefault();
                            e.stopPropagation();

                            $('.choice.pc .price-dropdown > .list > .price-item').removeClass('selected');
                            $(this).addClass('selected').parent().parent().removeClass('open').children('.caption').text($(this).text());

                            $("#text").text($(this).text());
                            try {
                                $(".text-btn").text(" " + (e.target.dataset.index === "1" ? ka.msg.auction.bidding : ka.msg.auction.bidding));
                            } catch (e) { }
                            try {
                                $.auction.priceNext = $(this).text().replace("KRW ", "");
                            } catch (e) { }
                            $('.bid-price-list').removeClass('block');
                        })
                        target.append(el);
                    });
                    $.each(target.find('li'), function (index, item) {
                        if (item.dataset.index === "1") item.click();
                    });
                }

                // UserHighestPre
                if (result.data.user_highest_pre != null) {
                    if (result.data.user_highest_pre.PriceBidPre > 0) {
                        $('.highest-bidder').show();
                        $('.my-highest-price').html($.stringUtils.comma(result.data.user_highest_pre.PriceBidPre));
                        $('.my-highest-date').html(result.data.user_highest_pre.RegDate);
                    }
                }

                // BidRemainTime
                console.log(result.data.bid_remain_time)

                if (result.data.bid_remain_time !== null) {
                    $.auction.setCountDown(".remainTime", new Date(result.data.bid_remain_time.replace(/-/ig, '/')));
                }

                $($.auction.bidListTarget + "-my").html("(" + myCount.toString() + ")");
                $($.auction.bidListTarget + "-total").html("(" + totalCount.toString() + ")");
                $($.auction.bidListTarget + "-total2").html("(" + ka.msg.auction.bidCount + ' <span class="num n2">' + totalCount.toString() + "</span>" + ka.msg.auction.bidCount2 + ")");
                $($.auction.bidListTarget + "-mb-total2").html("(" + ka.msg.auction.bidCount + ' <span class="num n2">' + totalCount.toString() + "</span>" + ka.msg.auction.bidCount2 + ")");
                $($.auction.bidListTarget + "-total3").html("(" + totalCount + ka.msg.auction.bidCount2 + ")");

                try {
                    if (parameters !== null && JSON.parse(parameters).mode === "refresh") {
                        $.commonUtils.notify(ka.msg.auction.refresh);
                    }
                } catch (e) { }
            }
        },

        getHighestTag: function () {
            return '<svg id="highestico" data-name="highestico" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 14" width="32px" height="14px" style="margin-right: 5px;"><defs><style>.highestico1{fill:#f44e05;}.chighestico2{isolation:isolate;}.highestico3{fill:#fff;}</style></defs><path id="사각형_435" data-name="사각형 435" class="highestico1" d="M4,0H28a4,4,0,0,1,4,4v6a4,4,0,0,1-4,4H4a4,4,0,0,1-4-4V4A4,4,0,0,1,4,0Z"/><g class="highestico2"><path class="highestico3" d="M8.2,8.5a12.25,12.25,0,0,0,1.9-.2l.1.6a25.3,25.3,0,0,1-4.6.3l-.2-.7H7.3V7.3h.8V8.5ZM5.6,6.9A2,2,0,0,0,7.3,5H5.7V4.3H7.4V3.5h.8v.8H9.8V5H8.2c0,.8.7,1.5,1.8,1.8l-.4.6A2.89,2.89,0,0,1,7.8,6,2.77,2.77,0,0,1,6,7.5Zm5.7-3.5v7.2h-.8V3.4Z"/><path class="highestico3" d="M19.3,9v.7H12.8V9h2.4V6.5H16V9ZM17.7,4.7H13.4V4h5.1v.7a15.53,15.53,0,0,1-.3,3.4L17.4,8a13.86,13.86,0,0,0,.3-3.3Z"/><path class="highestico3" d="M23.8,4.2a5.27,5.27,0,0,1-3.3,5.1L20,8.7a4.79,4.79,0,0,0,2.9-3.8H20.4V4.2Zm3,2.7H25.7v3.7h-.8V3.4h.8V6.3h1.1Z"/></g></svg>';
        },

        getMyBidTag: function () {
            return '<svg id="mybidico" data-name="mybidico" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 14" width="32px" height="14px" style="margin-right: 5px;"><defs><style>.mybidico1,.mybidico2{fill:none;}.mybidico2{stroke:#f44e05;}.mybidico3{isolation:isolate;}.mybidico4{fill:#f44e05;}</style></defs><g id="사각형_436" data-name="사각형 436"><path class="mybidico1" d="M4,0H28a4,4,0,0,1,4,4v6a4,4,0,0,1-4,4H4a4,4,0,0,1-4-4V4A4,4,0,0,1,4,0Z"/><path class="mybidico2" d="M4,.5H28A3.54,3.54,0,0,1,31.5,4v6A3.54,3.54,0,0,1,28,13.5H4A3.54,3.54,0,0,1,.5,10V4A3.54,3.54,0,0,1,4,.5Z"/></g><g class="cls-3"><path class="mybidico4" d="M4.9,4.1H6l1.1,3c.1.4.3.8.4,1.2h0a5.64,5.64,0,0,1,.4-1.2L9,4.1h1.1V10H9.2V7.1c0-.5.1-1.3.1-1.8h0L8.8,6.7l-1,2.8H7.2l-1-2.8L5.7,5.3h0c0,.5.1,1.3.1,1.8V10H5V4.1Z"/><path class="mybidico4" d="M13.4,10.3c-.3.9-.8,1.6-1.7,1.6a1.27,1.27,0,0,1-.6-.1l.2-.7c.1,0,.2.1.3.1a1,1,0,0,0,.9-.8l.1-.3L10.9,5.7h.9L12.6,8c.1.4.3.8.4,1.2h0L13.3,8,14,5.7h.9Z"/><path class="mybidico4" d="M15.9,4.1h1.8c1.2,0,2.1.4,2.1,1.4A1.34,1.34,0,0,1,19,6.8h0a1.33,1.33,0,0,1,1.2,1.4c0,1.2-1,1.7-2.3,1.7h-2Zm1.8,2.5c.9,0,1.3-.3,1.3-.9s-.4-.8-1.2-.8H17V6.6Zm.1,2.7c.9,0,1.4-.3,1.4-1s-.5-1-1.4-1h-.9v2Z"/><path class="mybidico4" d="M21.1,4.2c0-.3.2-.5.6-.5s.6.2.6.5a.56.56,0,0,1-.6.5C21.4,4.7,21.1,4.6,21.1,4.2Zm.1,1.4h.9V10h-.9Z"/><path class="mybidico4" d="M23.2,7.8a2.08,2.08,0,0,1,1.9-2.3,1.85,1.85,0,0,1,1.2.5V3.7h.9v6.4h-.8l-.1-.5h0a2,2,0,0,1-1.3.6C23.9,10.1,23.2,9.3,23.2,7.8Zm3,1V6.6a1.28,1.28,0,0,0-1-.4c-.6,0-1.1.6-1.1,1.5s.4,1.6,1.1,1.6A1.11,1.11,0,0,0,26.2,8.8Z"/></g></svg>';
        },

        setCurrentBidPrice: function (target, priceMax, priceAscend) {
            target.html("KRW " + $.stringUtils.comma(parseInt(priceMax.toString(), 10) + parseInt(priceAscend.toString(), 10)) + " 응찰");
        },

        /*------------------------------------------------------------------
        * @function:    checkBid
        * @description: 응찰 처리 함수 (신규UI)
        ------------------------------------------------------------------*/
        checkBid: function () {
            if ($.auction.priceNext === null) {
                $.commonUtils.alert(ka.msg.auction.bid_price_select);
            } else {
                // console.log("price_start - " + $.auction.priceStart + " / price_next - " + $.auction.priceNext + " / index 1 - " + $('.bid-price-list').find("[data-index='1']").attr("data-price"));

                if ($('.bid-price-list').find("[data-index='1']").attr("data-price").toString() === $.auction.priceNext.toString().replace(/,/g, "")) {
                    $("#btn-bidagree").attr("onclick", "$.auction.setBid()");
                } else {
                    $("#btn-bidagree").attr("onclick", "$.auction.setBidPre()");
                }
                onlinebidPopup();
            }
        },

        /*------------------------------------------------------------------
        * @function:    setBid
        * @description: 응찰 처리 함수
        ------------------------------------------------------------------*/
        setBid: function () {
            var param = {};
            param["work_uid"] = $.auction.workUid;
            param["auc_kind"] = $.auction.aucKind;
            param["auc_num"] = $.auction.aucNum;
            param["price_max"] = $.auction.priceMax.toString().replace(/,/ig, "");
            param["price_bid"] = $.auction.priceNext.toString().replace(/,/ig, "");
            param["bid_type"] = "bid";
            $.ajaxUtils.getApiData("/api/Auction/Bid/" + param["work_uid"], param, $.auction.setBidComplete);
        },

        /*------------------------------------------------------------------
        * @function:    setBidComplete
        * @parameter:   result (서버 처리 결과 json)
        *               parameters (서버로 전달한 json)
        * @description: 응찰 처리 결과 함수
        ------------------------------------------------------------------*/
        setBidComplete: function (result, parameters) {
            if ($.ajaxUtils.getResultCode(result) === "00") {
                // 응찰 처리 완료 - 그룹 메세지 처리 및 그룹 해제
                $.auction.sendBidMessage();
                $.auction.removeFromGroup();

                // UI 처리
                $('.bid-popup > img').click();
                $('.bidnone-all').hide();
                document.querySelector('.bid-popup.bp01').classList.remove('block');
                document.querySelector('.popupbg').classList.remove('block');

                existBidFlag = true;

                var bidType = JSON.parse(parameters).bid_type;
                if (bidType === "bid_pre") {
                    $.commonUtils.alertWithFn(ka.msg.auction.bid_pre_complete, 'success', "$.auction.getBidTotalList('');");
                } else {
                    $.commonUtils.alertWithFn(ka.msg.auction.bid_complte, 'success', "$.auction.getBidTotalList('');");
                }

                try {
                    if (useGA === "Y") {
                        gtag('event', 'Bid', { 'event_category': JSON.parse(parameters).auc_kind === "2" ? "Premium" : "Weekly" });
                    }
                } catch (e) { }
            } else {
                if (result.code.indexOf("ka.") > -1) {
                    $.commonUtils.alert(eval(result.code));
                } else {
                    $.commonUtils.alert(result.message);
                    if (result.code === "15") {
                        $.auction.getBidTotalList('refresh');
                    }
                }
            }
        },

        /*------------------------------------------------------------------
        * @function:    setBidPre
        * @description: 자동응찰 처리 함수
        ------------------------------------------------------------------*/
        setBidPre: function () {
            var param = {};
            param["work_uid"] = $.auction.workUid;
            param["auc_kind"] = $.auction.aucKind;
            param["auc_num"] = $.auction.aucNum;
            param["price_max"] = $.auction.priceMax.toString().replace(/,/ig, "");
            param["price_bid_pre"] = $.auction.priceNext.toString().replace(/,/ig, "");
            param["bid_type"] = "bid_pre";
            $.ajaxUtils.getApiData("/api/Auction/BidPre/" + param["work_uid"], param, $.auction.setBidComplete);
        },

        setCountDown: function (target, datetime) {
            $(target).countdown(datetime)
                .on('update.countdown', function (event) {
                    var diff = $.datetimeUtils.dateDiff('D', new Date(), datetime);
                    if (typeof isKor === "string" && isKor !== "True") {
                        $(this).html(diff < 1 ? event.strftime('%Hh %Mm %Ss') : event.strftime('%Dd %Hh %Mm'));
                    } else {
                        $(this).html(diff < 1 ? event.strftime('%H시간 %M분 %S초') : event.strftime('%D일 %H시간 %M분'));
                    }
                    if ($.datetimeUtils.dateDiff('D', new Date(), datetime) === 0 && $.datetimeUtils.dateDiff('H', new Date(), datetime) === 0 && $.datetimeUtils.dateDiff('M', new Date(), datetime) === 0) {
                        var remainSecond = $.datetimeUtils.dateDiff('S', new Date(), datetime);
                        if ((remainSecond === 27)) {
                            try {
                                if (((isMobile === "True" || $(window).width() < 1200) && $('#mobilebid').attr("class") === "block")
                                    || (isMobile !== "True" && $('.bid-list').css("display") === "block")) {
                                    $.commonUtils.setCookie("ka-bid-layer", "Y", 1);
                                }
                            } catch (e) { }
                            window.location.reload();
                        }
                    }
                })
                .on('finish.countdown', function (event) {
                    $(this).html('');
                    $('.btn-active-btn').html('');
                    $('.btn-end').show();
                    $('.deadline-hour').hide();
                    $('.bid-btn').html("<a href='#'>" + ka.msg.auction.bidEnd + "</a>");
                    $('.mobile-bidbtn').hide();
                    if (!$.auction.bidEndNoti) {
                        $.commonUtils.notify(ka.msg.auction.bidEnd);
                        $.auction.bidEndNoti = true;
                    }
                });
        },

        setBidStartCountDown: function (target, datetime) {
            $(target).countdown(datetime)
                .on('update.countdown', function (event) {
                    if (typeof isKor === "string" && isKor !== "True") {
                        $(this).html(event.strftime('Bidding opens in %Hh %Mm'));
                    } else {
                        $(this).html(event.strftime('응찰 대기시간 %H시간 %M분'));
                    }
                })
                .on('finish.countdown', function (event) {
                    window.location.reload();
                });
        }
    },

    $.wishlist = {
        login: function () {
            if (!$.loginUtils.isLogin()) {
                $.commonUtils.openLogin();
            } else {
                window.location.href = "/MyPage/Info";
            }
        },
        add: function (workUid, wishYN, reload) {
            reload = typeof reload === "boolean" ? reload : true;
            if (!$.loginUtils.isLogin()) {
                $.commonUtils.openLogin(ka.msg.auction.wishlistAdd);
            } else {
                var param = {};
                param["work_uid"] = workUid;
                if ($("#icon-w-" + workUid).length > 0) {
                    param["wish_yn"] = $("#icon-w-" + workUid).attr("data-wish") === "Y" ? "N" : "Y";
                } else {
                    param["wish_yn"] = wishYN;
                }
                var result = $.ajaxUtils.getApiData("/api/Member/SetWishWork/" + workUid, param, null, false);
                try {
                    if (result.code === "00") {
                        if ($("#icon-w-" + workUid).length > 0) {
                            if ($("#icon-w-" + workUid).attr("data-wish") === "Y") {
                                $("#icon-w-" + workUid).attr("data-wish", "N");
                                $("#icon-w-" + workUid).attr("class", "far fa-heart");
                            } else {
                                $("#icon-w-" + workUid).attr("data-wish", "Y");
                                $("#icon-w-" + workUid).attr("class", "fas fa-heart");
                            }
                        } else {
                            if (reload) {
                                if (typeof getList === "function") {
                                    getList();
                                } else {
                                    window.location.reload();
                                }
                            }
                        }
                        return true;
                    } else {
                        return false;
                    }
                } catch (e) { console.log(e.description); return false; }
            }
        },
        addNew: function (target) {
            if (!$.loginUtils.isLogin()) {
                $.commonUtils.openLogin(ka.msg.auction.wishlistAdd);
            } else {
                var param = {};
                param["work_uid"] = $.auction.workUid;
                param["wish_yn"] = $.auction.wishYN === "Y" ? "N" : "Y";
                var result = $.ajaxUtils.getApiData("/api/Member/SetWishWork/" + $.auction.workUid, param, null, false);
                try {
                    if (result.code === "00") {
                        $.auction.wishYN = param["wish_yn"];
                        $.wishlist.setDisplay(target);
                        if ($.auction.wishYN === "Y") {
                            $("#wishlilst-modal").css("display", "block");
                        }
                    }
                } catch (e) { console.log(e.description); }
            }
        },
        setDisplay: function (target) {
            if ($.auction.wishYN === "Y") {
                $(target).addClass("active");
            } else {
                $(target).removeClass("active");
            }
        }
    },

    $.history = {
        key: 'auctionWorkHistory',

        initList: function () {
            var storage = $.storageUtils;
            if (typeof storage.getItem($.history.key) === "string" && storage.getItem($.history.key) === "") {
                storage.setItem($.history.key, "");
            } else {
                var data = storage.getItem($.history.key);
                // if (!$.commonUtils.isNull(data)) {
                if (data !== "") {
                    var nowDate = new Date();
                    var itemList = JSON.parse(data);

                    var newItemList = new Array();
                    for (var i in itemList) {
                        var viewTime = new Date(itemList[i]["viewTime"]);
                        // if (nowDate.getTime() < viewTime.getTime()) {
                        if (nowDate.toISOString().split('T')[0] === viewTime.toISOString().split('T')[0]) {
                            newItemList.push(itemList[i]);
                        }
                    }

                    if (newItemList.length === 0) {
                        storage.setItem($.history.key, "");
                    } else {
                        storage.setItem($.history.key, JSON.stringify(newItemList));
                    }

                    $.history.render();
                }
            }
        },

        render: function () {
            var storage = $.storageUtils;
            if (typeof storage.getItem($.history.key) === "string" && storage.getItem($.history.key) !== "") {
                var itemList = JSON.parse(storage.getItem($.history.key));

                // 작품 상세 - 최근 본 작품
                var historyGrid = $("#history-grid");
                if (historyGrid.length > 0) {
                    for (var i in itemList) {
                        var el = $("<div />", {
                            "class": "mix mordern " + itemList[i]["aucKindName"],
                            "data-ref": "mix",
                            "data-artist": $.commonUtils.decodeHTML(itemList[i]["artistName"] + "&#xa;" + itemList[i]["workTitle"]),
                            "data-price": "클릭하여 상세정보를 확인하시기 바랍니다.",
                            "title": itemList[i]["aucKindName"] + ' Lot.' + itemList[i]["lotNum"],
                            "style": "cursor: pointer; margin: 5px 5px;",
                            "onclick": "window.open('/Auction/" + itemList[i]["aucKindName"] + "/" + itemList[i]["aucNum"] + "/" + itemList[i]["workUid"] + "', '_self');"
                        }).append(
                            $("<img />", { "style": "height: 200px; object-fit: cover;", "src": itemList[i]["thumFileName"] })
                        );
                        historyGrid.append(el);
                    }
                }

                // Floating Menu - History
                var historyFloating = $(".fm-history");
                if (historyFloating.length > 0) {
                    historyFloating.find(".fm-mask").css("height", parseInt(window.innerHeight / 2, 10).toString() + "px");
                    var target = historyFloating.find(".fm-label");
                    for (var j in itemList) {
                        var img = $("<div />", { "style": "padding: 3px;", "onclick": "window.open('/Auction/" + itemList[j]["aucKindName"] + "/" + itemList[j]["aucNum"] + "/" + itemList[j]["workUid"] + "', '_self');" }).append(
                            $("<img />", { "style": "max-width: 50px;", "src": itemList[j]["thumFileName"] }));
                        target.append(img);
                    }
                }
            }
        },

        add: function (data) {
            var storage = $.storageUtils;
            if (storage.isAvailable()) {
                data["viewTime"] = new Date();
                if ($.commonUtils.isNull(storage.getItem($.history.key))) {
                    var itemList = new Array();
                    itemList.push(data);
                    storage.setItem($.history.key, JSON.stringify(itemList));
                } else {
                    var json = JSON.parse(storage.getItem($.history.key));
                    var duplicate = false;
                    for (var i in json) {
                        if (json[i]["workUid"] === data["workUid"]) {
                            duplicate = true;
                        }
                    }
                    if (!duplicate) {
                        json.push(data);
                        storage.setItem($.history.key, JSON.stringify(json));
                    }
                }
            }
        }
    },

    $.bid = {
        redirectMajorBid: function (uid) {
            if (!$.loginUtils.isLogin()) {
                $.commonUtils.openLogin(ka.msg.auction.placebid2);
            } else {
                window.location.href = "/Auction/BidApplication/" + uid.toString();
            }
        }
    },

    $.virtualExhibition = {
        isActive: function (id) {
            switch (id) {
                case "kjpark":
                case "zang0804":
                case "qordmswl5631":
                    return true;
                default:
                    return false;
            }
        }
    }

})(window.jQuery);