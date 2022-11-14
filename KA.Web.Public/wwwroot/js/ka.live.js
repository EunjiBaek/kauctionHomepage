'use strict';
const LiveApiHost = "/api/Live";

var $Template = {
    tp_snb_right_bidhst: $("#tp_snb_right_bidhst")
    ,
    tp_svg_mybid: $("#tp_svg_mybid")
    ,
    tp_svg_sold: $("#tp_svg_sold")
    ,
    tp_svg_sold_ko: $("#tp_svg_sold_ko")
    ,
    tp_svg_live: $("#tp_svg_live")
    ,

    //get tp_snb_right_bidhst() {
    //    return $("#tp_snb_right_bidhst");
    //},
    //get tp_svg_mybid() {
    //    return $("#tp_svg_mybid");
    //},
    //get tp_svg_sold() {
    //    return $("#tp_svg_sold");
    //},
    //get tp_svg_sold_ko() {
    //    return $("#tp_svg_sold_ko");
    //},
    //get tp_svg_live() {
    //    return $("#tp_svg_live");
    //},
    PriceComma: function (val, render) {
        return Number($("<span />", { html: render(val) }).text()).toLocaleString();
    }
}

var LiveObj = {
    get $lot_num() {
        return $("#lot_num");
    },
    get $bid_price() {
        return $("#bid_price");
    },
    get $bid_increase_price() {
        return $("#bid_increase_price");
    },
    get $notice_list() {
        return $("#notice_list");
    },
    get $ul_snb_right_bidlist() {
        return $("#ul_snb_right_bidlist");
    },
    get $div_snb_left() {
        return $("#div_snb_left");
    },
    get $div_snb_left_tabs() {
        return $("#div_snb_left_tabs");
    },
    get currency() {
        try {
            var currency = localStorage.getItem("CURRENCY") || "KRW";
            return {
                isKRW: currency === "KRW"
                , isUSD: currency === "USD"
                , isJPY: currency === "JPY"
                , isHKD: currency === "HKD"
                , isCNY: currency === "CNY"
                , isEUR: currency === "EUR"
            };
        }
        finally {
            currency = null;
        }
    },
    get mypaddle() {
        return {
            isMyPaddleNum: AuctionData.Table.length > 0 ? (AuctionData.Table[0].my_paddle_num > 0) : false,
            my_paddle_num: AuctionData.Table.length > 0 ? AuctionData.Table[0].my_paddle_num : 0,
            isBidType: AuctionData.Table.length > 0 ? AuctionData.Table[0].my_bid_type === 5 : false,
            my_bid_type: AuctionData.Table.length > 0 ? AuctionData.Table[0].my_bid_type : 0
        };
    },
    get auctionstat() {
        try {

            var data = AuctionData.Table[0] || {};

            return {
                auc_date: data.auc_date
                , isWait: data.auc_stat_cd === "W"
                , isAllow: data.auc_stat_cd === "A"
                , isStart: data.auc_stat_cd === "S"
                , isPause: data.auc_stat_cd === "P"
                , isFinish: data.auc_stat_cd === "F"
                , isEnd: data.auc_stat_cd === "E"
                , videoOnOffImg: $(".video").is(":visible") ? "/img/live/invalid-name4.png" : "/img/live/invalid-name5.png"
            }
        }
        finally {
            data = null;
        }
    },
    get videoType() {        
        return enabledUnrealHTML5VideoPlayer();
    }
}

var LiveFunc = {
    fn_param: function () {
        var script = $("script");
        script = script[script.length - 1].src
            .replace(/^[^\?]+\?/, '')
            .replace(/#.+$/, '')
            .split('&')
        var queries = {}, query;
        while (script.length) {
            query = script.shift().split('=');
            queries[query[0]] = query[1];
        }
        return queries;
    },
    fn_template_set: function ($template, data, $target) {        
        var rendered = Mustache.render($template.html(), data);
        $target.html("").append(rendered);
    },
    fn_template_append: function ($template, data, $target) {
        var rendered = Mustache.render($template.html(), data);
        $target.append(rendered);
    },
    fn_template_prepend: function ($template, data, $target) {
        var rendered = Mustache.render($template.html(), data);
        $target.prepend(rendered);
    },
    fn_GetDayName: function (day) {
        switch (day) {
            case 0:
                day = '일';
                break;
            case 1:
                day = '월';
                break;
            case 2:
                day = '화';
                break;
            case 3:
                day = '수';
                break;
            case 4:
                day = '목';
                break;
            case 5:
                day = '금';
                break;
            case 6:
                day = '토';
                break;
        }

        return day;
    },
    fn_hasClass: function (e, n) {
        return e.classList.contains(n);
    },

    fn_init: function () {
        //console.time('fn_init')

        new Promise(LiveFunc.fn_header)
            .then(new Promise(fn_ws_open))
            .then(new Promise(LiveFunc.fn_snb_left_tabs))
            .then(new Promise(LiveFunc.fn_snb_left_conts))
            ;

        LiveFunc.fn_notice_list();

        //console.timeEnd('fn_init');
    },

    fn_landing: function () {
        if (LiveObj.auctionstat.isWait || LiveObj.auctionstat.auc_date === undefined) {
            $(".preparing").show();
            $("#mid-content").hide();
        } else if (LiveObj.auctionstat.isEnd) {
            $(".preparing").hide();
            $("#mid-content").hide();
        } else {
            $(".preparing").hide();
            $("#mid-content").show();
        }
    },
    fn_header: function () {
        try {
            var data = { rows: AuctionData.Table };

            $.extend(data, LiveObj.currency, LiveObj.mypaddle);

            //옥션정보 바인딩
            LiveFunc.fn_template_set($("#tp_header"), data, $("#top"));
        }
        finally {
            data = null;
        }
    },

    fn_snb_left_tabs: function () {

        try {
            var data = { rows: AuctionData.Table1 };

            LiveFunc.fn_template_set($("#tp_snb_left_tabs"), data, $("#div_snb_left_tabs"))
        }
        finally {
            data = null;
        }
    },
    fn_snb_left_conts: function () {        
        try {
            let currLot = LiveObj.$lot_num.val() || AuctionData.Table3[0].lot_num;

            //카테고리별 작품리스트
            AuctionData.Table1.forEach(function (e, i) {
                var lotinfos = AuctionData.Table2.reduce(function (rslt, curr) {
                    if (typeof curr["t_seq"] === undefined)
                        curr["t_seq"] = "999";
                    rslt[curr["t_seq"]] = rslt[curr["t_seq"]] || [];
                    rslt[curr["t_seq"]].push(curr);
                    return rslt;
                }, {});
                
                let index = lotinfos[e.t_seq].findIndex(lot => lot.lot_num === currLot) + 1;

                let page_no = Math.ceil(index / lot_page_size);                                

                e.lotinfos = lotinfos[e.t_seq].slice((page_no - 1) * lot_page_size, page_no * lot_page_size);
                e.page_no = page_no;
                e.page_no_list = [];
                e.page_no_list.push(page_no);
                lotinfos = null;
            });           

            var data = {
                rows: AuctionData.Table1
                , PriceComma: function () {
                    return $Template.PriceComma;
                }
            };
            LiveFunc.fn_template_set($("#tp_snb_left"), data, $("#div_snb_left"));
        }
        finally {
            data = null;
            AuctionData.Table1.forEach(function (t) { t.lotinfos = null; });
        }
    },
    fn_snb_left_conts_paging: function (tp) {
        //console.time("fn_snb_left_conts_paging");
      
        try {
            var pSize = lot_page_size
                , curr_t_seq = $("#div_snb_left_tabs li.active > a").data("t_seq")
                , max_page_no = 1
                , $t_seq = $("#t_seq_" + curr_t_seq + ">ul>li");
            ;

            var rows = AuctionData.Table2.filter(function (e) { return e.t_seq == curr_t_seq })

            if (rows <= $t_seq.length) {
                console.log("remove scroll");
                LiveObj.$div_snb_left.get(0).removeEventListener("scroll", null);
            }

            max_page_no = Math.ceil(rows.length / pSize);
            
            var pageInfo = AuctionData.Table1.filter((function (e) { return e.t_seq == curr_t_seq }))[0];

            if ($t_seq.length < 1) pageInfo.page_no = 0;

            if(tp === "up")
                pageInfo.page_no -= 1;
            else 
                pageInfo.page_no += 1;

            if (pageInfo.page_no > max_page_no) {
                pageInfo.page_no = max_page_no;
                return;
            }
            if (pageInfo.page_no < 1) {
                pageInfo.page_no = 1;
                return;;
            }

            if (pageInfo.page_no_list.indexOf(pageInfo.page_no) > -1)
                return;
            else
                pageInfo.page_no_list.push(pageInfo.page_no);

            var data = {
                rows: rows.slice((pageInfo.page_no - 1) * pSize, pSize * pageInfo.page_no)
                , PriceComma: function () {
                    return $Template.PriceComma;
                }
            };            
            if (tp === "up")
                LiveFunc.fn_template_prepend($("#tp_snb_left_row"), data, $("#t_seq_" + curr_t_seq + "> ul"));
            else 
                LiveFunc.fn_template_append($("#tp_snb_left_row"), data, $("#t_seq_" + curr_t_seq + "> ul"));
        }
        finally {
            data = null;            
            //console.timeEnd("fn_snb_left_conts_paging");
        }
    },
    fn_snb_left_conts_paging_up: function () {
        LiveFunc.fn_snb_left_conts_paging("up");
    },

    fn_snb_mid_top: function (data) {
        
        /* 옥션 경매 상태 변경*/
        if (AuctionData.Table.length > 0 && data != null && data.auc_stat_cd != null) {

            AuctionData.Table[0].auc_stat_cd = data.auc_stat_cd;

            if (AuctionData.auc_stat_cd !== data.auc_stat_cd)
                this.fn_landing();
        }           

        if (AuctionData.auc_stat_cd !== data.auc_stat_cd) {
            LiveFunc.fn_template_set($("#tp_snb_mid_top"), LiveObj.auctionstat, $("#div_snb_mid_top"));

            if (LiveObj.auctionstat.isStart || LiveObj.auctionstat.isAllow || LiveObj.auctionstat.isPause) {
                if (!isVideoConnect) {
                    LiveFunc.fn_template_set($("#tp_player_message"), { messageHtml: EtcMsg.VideoWait, isShowPlay: true }, $(".video"));
                }
            } else if (LiveObj.auctionstat.isWait) {
                isVideoConnect = false;
                LiveFunc.fn_template_set($("#tp_player_message"), { messageHtml: EtcMsg.VideoPause, isShowStop: true }, $(".video"));
            } else {
                isVideoConnect = false;
                LiveFunc.fn_template_set($("#tp_player_message"), { messageHtml: EtcMsg.VideoFinish, isShowThank: true }, $(".video"));
            }
        }
    },
    fn_mybid_list: function () {

        //console.log("fn_mybid_list");

        $.extend(Param, { page_no: 1, page_size: 100 });
        $.ajax({
            type: "post",
            async: true,
            url: LiveApiHost + "/usp_Live_Auc_Bidding_Hst_SelectForMyBid",
            datatype: "json",
            contentType: "application/json;charset=UTF-8",
            data: JSON.stringify(Param),
            success: function (r) {
                try {
                    var data = {
                        Table: r.data.Table
                        , PriceComma: function () {
                            return $Template.PriceComma;
                        }
                        , DateTme: function () {
                            return function (val, render) {
                                var str = $("<span />", { html: render(val) }).text();
                                var date = new Date(str);
                                return (
                                    date.getFullYear()
                                    + '.' + ((date.getMonth() + 1) < 10 ? ('0' + (date.getMonth() + 1)) : (date.getMonth() + 1))
                                    + '.' + (date.getDate() < 10 ? ('0' + date.getDate()) : date.getDate())
                                    + '<br />'
                                    + ' ' + (date.getHours() < 10 ? ('0' + date.getHours()) : date.getHours())
                                    + ':' + (date.getMinutes() < 10 ? ('0' + date.getMinutes()) : date.getMinutes())
                                    + ':' + (date.getSeconds() < 10 ? ('0' + date.getSeconds()) : date.getSeconds())

                                )
                            };
                        }
                        , ConvertBidStatName: function () {
                            return function (val, render) {
                                var str = $("<span />", { html: render(val) }).text();
                                return BidStatCd[str] || "";
                            };
                        }
                    };
                    LiveFunc.fn_template_set($("#tp_mybidlist"), data, $("#tg_mybidlist"));
                }
                finally {
                    data = null;
                }
            },
            timeout: 2000
        });
    },
    fn_wish_list: function () {

        //console.log("fn_wish_list");

        $.ajax({
            type: "post",
            async: true,
            url: LiveApiHost + "/usp_Live_Mem_Wish_Info_Select",
            datatype: "json",
            contentType: "application/json;charset=UTF-8",
            data: JSON.stringify(Param),
            success: function (r) {
                try {
                    var data = {
                        hasRows: r.data.Table.length > 0
                        , Rows: r.data.Table
                        , PriceComma: function () {
                            return $Template.PriceComma;
                        }
                        , ImagePath: function () {
                            return function (val, render) {
                                var str = $("<span />", { html: render(val) }).text();
                                return (
                                    (AuctionData.Table2.filter(function (e) {
                                        return e.work_seq == str;
                                    })[0] || { img_file_name: "" }).img_file_name
                                )
                            };
                        }
                    };

                    LiveFunc.fn_template_set($("#tp_mywishlist"), data, $("#tg_mywishlist"));
                    $("div.mid-bottom>div.midbt-btn>ul>li:eq(2)").click();
                }
                finally {
                    data = null;
                }
            },
            timeout: 2000
        });
    },
    fn_wish_insert: function () {
        
        var work_seq = event.target.dataset.work_seq;

        $.extend(Param, { work_seq: work_seq });

        $.ajax({
            type: "post",
            async: true,
            url: LiveApiHost + "/usp_Live_Mem_Wish_Info_Insert",
            datatype: "json",
            contentType: "application/json;charset=UTF-8",
            data: JSON.stringify(Param),
            success: function (r) {
                var use_yn = r.data.Table[0].use_yn || 'N';

                var $t = $("div.like[data-work_seq=" + work_seq + "]>a>img");

                if (use_yn === "Y") {
                    $t.attr({ "src": "/img/live/invalid-name2.png" });
                }
                else {
                    $t.attr({ "src": "/img/live/invalid-name.png" });
                }
            },
            complete: LiveFunc.fn_wish_list,
            timeout: 10000
        });
    },
    fn_notice_list: function () {
        $.ajax({
            type: "post",
            async: false,
            url: LiveApiHost + "/usp_Live_Auc_Notice_Info_Select",
            datatype: "json",
            contentType: "application/json;charset=UTF-8",
            data: JSON.stringify(Param),
            success: function (r) {
                try {
                    var $rows = $("<ul />");
                    $.each(r.data.Table, function (i, e) {
                        $rows.append(
                            "<li data-noti_seq={{noti_seq}} value='{{noti_memo}}'>{{noti_memo}}</li>".replace("{{noti_seq}}", e.noti_seq).replace("{{noti_memo}}", e.noti_memo).replace("{{noti_memo}}", e.noti_memo)
                        );
                    });

                    LiveObj.$notice_list.html("").append($rows);
                }
                finally {
                    $rows = null;
                }
            },
            complete: function () {
                clearInterval(noticeDelay);
                noticeDelay = setInterval(LiveFunc.fn_notice_list, 30000);
               
                clearInterval(noticeSlideDelay);
                noticeSlideDelay = setInterval(LiveFunc.fn_notice_slide, 5000);

                LiveObj.$notice_list.find("ul li").hover(function () {
                    clearInterval(noticeSlideDelay);

                    $(".notice-tooltip").css("display", "block");
                    $(".notice-all .notice-tooltip > p").text($(this).attr("value"));
                }, function () {
                    noticeSlideDelay = setInterval(LiveFunc.fn_notice_slide, 5000);
                    $(".notice-tooltip").css("display", "none");
                });
            },
            timeout: 2000
        });
    },   

    fn_selected_lotinfo: function (e) {        

        //console.time("fn_selected_lotinfo");
        //console.log("scope", this);
        //console.log("event", e);        

        try {
            selected_call_cnt++;
            
            var $this = e.type === "load" || e.type === "change" ? this : e.target;           

            $this = e.target.id === "curr_lot_num" || (e.target.nodeName === "A" && e.target.className === "check") ? LiveObj.$lot_num.get(0) : $this;

            var lot_num = $this.dataset.lot_num || $this.value,
                data = AuctionData.Table2.filter(function (e) {
                    return e.lot_num == lot_num;
                })
                ;

            data.PriceComma = function () {
                return $Template.PriceComma;
            }
            data.auc_num = Param.auc_num;
            data.isCurrLot = lot_num === document.getElementById("lot_num").value;

            $.extend(data, LiveObj.currency);

            //Lot정보
            LiveFunc.fn_template_set($("#tp_lotinfo"), data, $("#tg_selected_lotinfo"));

            //탭클릭
            if (data.length > 0) {                
                $("#div_snb_left_tabs>ul>li").find("a[data-t_seq=" + data[0].t_seq + "]").click();
                $("div.mid-bottom>div.midbt-btn>ul>li:eq(0)").click();
            }

            //Live
            if ($this.id === "lot_num") {

                var $div_snb_left = LiveObj.$div_snb_left.find("ul"),
                    $lot = $div_snb_left.find("li[data-lot_num=" + lot_num + "]"),
                    scrollTop = 0
                    ;

                //기존 LOT 원복
                $div_snb_left.find("li")
                    .removeClass("live")
                    .find("h6>svg")
                    .remove();

                //현재 LOT 선택
                $lot.addClass("live")
                    .find("div.works-desc>h6")
                    .append($Template.tp_svg_live.html())

                //스크롤
                for (var i = 1; i < $lot.index(); i++) {
                    scrollTop += $lot.parent().parent().find("ul>li").eq(i).outerHeight();
                }

                //스크롤 이동 및 지연로딩
                $(".tap-content").animate({ scrollTop: scrollTop, duration: 400 }, null);

                //if (selected_call_cnt === 1)
                //{   
                //    new Promise(function () { LiveFunc.fn_lazyload(true) }).then(new Promise(function () {
                //        $(".tap-content").scrollTop(scrollTop);
                //    }));
                //}   
                //else
                //    $(".tap-content").animate({ scrollTop: scrollTop, duration: 400 }, null);
            }
        }
        finally {
            data = null;            

            //console.timeEnd("fn_selected_lotinfo");
        }
    },
    fn_bidding: function () {
        event.preventDefault();
        event.stopPropagation();

        $.extend(Param, { paddle_num: AuctionData.Table[0].my_paddle_num, bid_price: this.dataset.next_bid_price });

        clearTimeout(keyDelay);

        keyDelay = setTimeout(function () {

            console.log("bidding param:",JSON.stringify(Param));

            $.ajax({
                type: "post",
                async: false,
                url: LiveApiHost + "/usp_Live_Auc_Bidding_Hst_InsertProc",
                datatype: "json",
                contentType: "application/json;charset=UTF-8",
                data: JSON.stringify(Param),
                success: function (r) {
                    var data = r.data.Table[0];
                    LiveFunc.fn_bidding_confirm(data);
                },
                complete: LiveFunc.fn_mybid_list,
                timeout: 1000
            });
        }, 200);
    },
    fn_bidding_confirm: function (data) {

        if (data.code == "99") {
            try {
                var opt = { titleHtml: "", okText: Btn.OK, cancelText: Btn.CANCEL };

                opt.cancelText = "";
                opt.messageHtml = "<p>" + RsltCd[data.msg].replace("{{bid_price}}", Number(data.bid_price).toLocaleString()) + "</p>";
                opt.completed = function (r) {
                    if (r) {
                        //LiveFunc.fn_bidding();                    
                    }
                    return r;
                }

                $.layerConfirm(opt);
            }
            finally {
                opt = null;
            }
        }
        else {
            toastr["success"](RsltCd.SUCC.replace("{{lot_num}}", data.lot_num).replace("{{bid_price}}", Number(data.bid_price).toLocaleString()).replace("{{reg_date}}", data.reg_date.toLocaleString()));
        }
    },
    fn_curr_lotinfo_complete: function (res) {

        //console.log("fn_curr_lotinfo_complete", event.currentTarget);
        
        try {

            var data = res.data.Table[0],
                $lot_num = LiveObj.$lot_num;

            if (ticks >= data.call_date) {                                
                return;
            }

            ticks = data.call_date;

            /* 옥션상태 변경*/
            LiveFunc.fn_snb_mid_top(data);

            /* 응찰 마감 경고 메시지 */
            if (AuctionData.is_fair_warning !== data.is_fair_warning) {
                try {
                    var $tg = LiveObj.$ul_snb_right_bidlist;
                    if (data.is_fair_warning) {
                        if ($tg.find(".deadline").length < 1) {
                            $tg.prepend("<li class=\"deadline\"><p>{{EtcMsg.FairWarning}}</p><img src=\"/img/live/remainico.gif\"></div>".replace("{{EtcMsg.FairWarning}}", EtcMsg.FairWarning));
                        }
                    }
                    AuctionData.is_fair_warning = data.is_fair_warning;
                }
                finally {
                    $tg = null;
                }
            }

            //현재파라미터에 랏 바인드
            $.extend(Param, { lot_num: $lot_num.val() });
            //console.log(Param);

            if (        AuctionData.auc_stat_cd                         !== data.auc_stat_cd
                    ||  AuctionData.lot_stat_cd                         !== data.lot_stat_cd
                    ||  AuctionData.lot_num                             !== data.lot_num
                    ||  AuctionData.bid_hst_seq                         !== data.bid_hst_seq
                    ||  (AuctionData.claim_bid_hst_seq || 0).toString() !== (data.claim_bid_hst_seq || 0).toString()
                    ||  AuctionData.successful_bid                      !== data.successful_bid
                    ||  AuctionData.bid_increase_price                  !== data.bid_increase_price
                    ||  AuctionData.bid_price_resv                      !== data.bid_price_resv
                    ||  AuctionData.currency                            !== (localStorage.getItem("CURRENCY") || "KRW")
            ) {


                /* 낙찰취소 토스트 */
                if (AuctionData.lot_num === data.lot_num && AuctionData.successful_bid == "0" && data.successful_bid == "1") {
                    toastr["info"](RsltCd.NOTI_CHG_SUCCBID_CANCEL);
                }

                AuctionData.lot_num = data.lot_num;

                /* 낙찰변경 */
                if (AuctionData.successful_bid !== data.successful_bid || AuctionData.lot_stat_cd !== data.lot_stat_cd) {

                    $.extend(Param, { lot_num: data.lot_num });

                    $.ajax({
                        type: "post",
                        async: true,
                        url: LiveApiHost + "/usp_Live_auction_work_Select",
                        datatype: "json",
                        contentType: "application/json;charset=UTF-8",
                        data: JSON.stringify(Param),
                        success: function (r) {

                            //var auctionData = AuctionData.Table1.filter(function (t) { return t.t_seq === r.data.Table[0].t_seq; })[0].lotinfos.filter(function (e) { return e.lot_num === AuctionData.lot_num; })[0]
                            //auctionData.successful_bid = r.data.Table[0].successful_bid;
                            //auctionData.successful_bid_price = r.data.Table[0].successful_bid_price;                            

                            AuctionData.Table2.filter(function (e) { return e.lot_num === AuctionData.lot_num; })[0].successful_bid = r.data.Table[0].successful_bid;
                            AuctionData.Table2.filter(function (e) { return e.lot_num === AuctionData.lot_num; })[0].successful_bid_price = r.data.Table[0].successful_bid_price;

                            try {
                                var $finishLot = LiveObj.$div_snb_left.find("li[data-lot_num='" + AuctionData.lot_num + "']");
                                if (!r.data.Table[0].successful_bid) {
                                    $finishLot.addClass("succ");
                                    $finishLot
                                        .find("p.price[data-lot_num= '" + AuctionData.lot_num + "']")
                                        .html("{{Mid_Left_SuccessBidPrice}} KRW {{successful_bid_price}}"
                                            .replace("{{Mid_Left_SuccessBidPrice}}", "낙찰가")
                                            .replace("{{successful_bid_price}}", r.data.Table[0].successful_bid_price.toLocaleString())
                                        );
                                }
                                else {
                                    $finishLot.removeClass("succ");
                                    $finishLot
                                        .find("p.price[data-lot_num= '" + AuctionData.lot_num + "']")
                                        .html(
                                            "KRW {{w_price}} ~ {{w_high_price}}"
                                                .replace("{{w_price}}", Number(r.data.Table[0].w_price).toLocaleString())
                                                .replace("{{w_high_price}}", Number(r.data.Table[0].w_high_price).toLocaleString())
                                        );
                                }
                            }
                            finally {
                                $finishLot = null;
                            }   
                        },
                        complete: function () {
                            //LiveFunc.fn_snb_left_conts();
                            LiveObj.$lot_num.data("lot_num", AuctionData.lot_num).val(AuctionData.lot_num).trigger("change");
                        },
                        timeout: 2000
                    }).then(LiveFunc.fn_mybid_list);
                }

                AuctionData.auc_stat_cd = data.auc_stat_cd;
                AuctionData.lot_stat_cd = data.lot_stat_cd;
                AuctionData.bid_hst_seq = data.bid_hst_seq;
                AuctionData.claim_bid_hst_seq = data.claim_bid_hst_seq;
                AuctionData.successful_bid = data.successful_bid;
                AuctionData.bid_increase_price = data.bid_increase_price;
                AuctionData.bid_price_resv = data.bid_price_resv;
                AuctionData.currency = (localStorage.getItem("CURRENCY") || "KRW");

                /* 응찰정보 변경 */
                data.lotinfo = AuctionData.Table2.filter(function (e) {
                    return e.lot_num === data.lot_num;
                });
                data.isWait = data.lot_stat_cd === "W";
                data.isAbsentee = data.lot_stat_cd === "A";
                data.isPause = data.lot_stat_cd === "P";
                data.isFinish = data.lot_stat_cd === "F";
                data.isStart = data.lot_stat_cd === "S";
                data.isSuccessBidIng = data.lot_stat_cd === "N";
                data.isPreBid = data.pre_bid_proc_yn;
                data.isSuccessPaddleNum = data.successful_paddle_num > 0;
                data.isBidRestricted = data.lotinfo[0].t_seq === 101 && LiveObj.mypaddle.my_bid_type === 6 ? false : true;
                data.isBidHst = $("#ul_snb_right_bidlist li[data-bid_hst_seq]").not(".cancel").length > 0;

                data.PriceComma = function () {
                    return $Template.PriceComma;
                }

                if (data.auc_stat_cd === "W" || data.auc_stat_cd === "P") {
                    data.isPause = false;
                    data.isWait = true;
                    data.isFinish = false;
                    data.isStart = false;
                }
                else if (data.auc_stat_cd === "F") {
                    data.isPause = false;
                    data.isWait = false;
                    data.isFinish = true;
                    data.isStart = false;
                }

                /* LOT 정보 바인드*/
                $.extend(data, LiveObj.currency, LiveObj.mypaddle);
                LiveFunc.fn_template_set($("#tp_snb_right"), data, $("#div_snb_right"));


                /* LOT 정보 변경 */
                if ($lot_num.val() != data.lot_num) {
                    /* LOT 정보 변경 */
                    //$lot_num.data("lot_num", data.lot_num).val(data.lot_num).trigger("change");
                    $lot_num.data("lot_num", data.lot_num).val(data.lot_num);

                    /* 응찰내역 변경 */
                    var $rows = LiveObj.$ul_snb_right_bidlist;
                    $rows.html("").html("<li><div style='width:100%;text-align:center;'><p>" + EtcMsg.NoBids + "</div ></li > ");
                }
                else {

                    if (LiveObj.mypaddle.my_paddle_num > 0) {

                        /* 변경정보 토스트 */
                        var deletes = res.data.Table1.filter(function (e) {
                            return e.bid_stat_cd === "DEL" && e.paddle_num === LiveObj.mypaddle.my_paddle_num;
                        });

                        var cancels = res.data.Table1.filter(function (e) {
                            return e.bid_stat_cd === "CNL" && e.paddle_num === LiveObj.mypaddle.my_paddle_num;
                        });

                        if (deletes.length > 0) {
                            $.each(deletes, function (i, e) {
                                toastr["error"](e.bid_noti_memo.length > 0 ? e.bid_noti_memo : RsltCd.NOTI_CHG_INCREASE_PRC.replace("{{bid_price}}", Number(data.bid_price).toLocaleString()));
                            });

                            LiveFunc.fn_mybid_list();
                        }

                        if (cancels.length > 0) {

                            $.each(cancels, function (i, e) {
                                toastr["error"](e.bid_noti_memo.length > 0 ? e.bid_noti_memo : RsltCd.NOTI_CHG_MY_BID.replace("{{bid_price}}", Number(data.bid_price).toLocaleString()));
                            });

                            LiveFunc.fn_mybid_list();
                        }
                    }
                }
            }
        }
        finally {
            data = null;
        }
    },
    fn_curr_lotinfo_bidhst_complete: function (res) {

        try {
            var data = res.data,
                $rows = LiveObj.$ul_snb_right_bidlist,
                bid_hst_seq = Math.max.apply(null, data.Table.map(function (v) { return v.bid_hst_seq; }))
                ;
            
            if (($rows.find(".active").data("bid_hst_seq") || 0) != (1 / bid_hst_seq === 0 ? 0 : bid_hst_seq)) {                
                $rows.html("");
                //return;
            }

            //$rows.html("");
            $.each(data.Table.reverse(), function (i, e) {
                var html = $Template.tp_snb_right_bidhst.html();
                html = html
                    .replace("{{mybid}}", (e.paddle_num == ("#" + LiveObj.mypaddle.my_paddle_num)) ? "mybid" : "")
                    .replace("{{bid_hst_seq}}", e.bid_hst_seq)
                    .replace("{{org_bid_price}}", e.bid_price)
                    .replace("{{bid_reg_date}}", e.bid_reg_date)
                    .replace("{{successful_bid}}", (!e.successful_bid ? "Last-sold" : ""))
                    .replace("{{paddle_num}}", e.paddle_num)
                    .replace("{{bid_type_name}}", BidTypeCd[e.bid_type_cd] || "")
                    .replace("{{bid_stat_name}}", e.bid_type_cd !== "ONL" ? (BidStatCd[e.bid_stat_cd] || "") : "")
                    .replace("{{successful_ko_bid_img}}", (!e.successful_bid ? $Template.tp_svg_sold_ko.html() : ""))
                    .replace("{{successful_en_bid_img}}", (!e.successful_bid ? $Template.tp_svg_sold.html() : ""))
                    .replace("{{bid_price}}", e.bid_price.toLocaleString())
                    .replace("{{mybidlayer}}", (e.paddle_num == ("#" + LiveObj.mypaddle.my_paddle_num)) ? $Template.tp_svg_mybid.html() : "")
                    .replace("{{isCancel}}", e.isCancel ? "cancel" : "")
                    ;

                var $row = $rows.find("li[data-bid_hst_seq=" + e.bid_hst_seq + "]");

                if ($row.length < 1) {
                    $rows.prepend(html);
                }
                else {
                    $rows.find("li[data-bid_hst_seq=" + e.bid_hst_seq + "]").replaceWith(function () {
                        return html;
                    })
                }
            })

            if (data.Table.length < 1) {
                //console.log($rows.find("li.deadline").parent.html());
                $rows.html(($rows.find("li.deadline").parent.html() || "") + "<li><div style='width:100%;text-align:center;'><p>" + EtcMsg.NoBids + "</div ></li >");
            }

            $rows.find("li[data-bid_hst_seq]:eq(0)").addClass("active");
        }
        finally {
            data = null;
        }
    },
    fn_bidhst_list: function () {
        
        $("#divBidHstList").css("display", "block");

        $.extend(Param, { lot_num: LiveObj.$lot_num.val(), page_no: 1, page_size: 100 });

        $.ajax({
            type: "post",
            async: true,
            url: LiveApiHost + "/usp_Live_Auc_Bidding_Hst_SelectForPage",
            datatype: "json",
            contentType: "application/json;charset=UTF-8",
            data: JSON.stringify(Param),
            success: function (r) {

                var data = {
                    Table: r.data.Table
                    , total_cnt: r.data.Table.length > 0 ? r.data.Table[0].total_cnt : 0
                    , lot_num: r.data.Table.length > 0 ? r.data.Table[0].lot_num : LiveObj.$lot_num.val()
                    , PriceComma: function () {
                        return $Template.PriceComma;
                    }
                    , ConvertRegDate: function () {
                        return function (val, render) {
                            var str = $("<span />", { html: render(val) }).text();
                            return str;
                        };
                    }
                    , ConvertBidTypeName: function () {
                        return function (val, render) {
                            var str = $("<span />", { html: render(val) }).text();
                            return BidTypeCd[str] || "";
                        };
                    }
                    , ConvertBidStatName: function () {
                        return function (val, render) {
                            var str = $("<span />", { html: render(val) }).text();
                            return BidStatCd[str] || "";
                        };
                    }
                    , ConvertMyBid: function () {
                        return function (val, render) {
                            var str = $("<span />", { html: render(val) }).text();
                            return str === ("#" + LiveObj.mypaddle.my_paddle_num) ? $Template.tp_svg_mybid.html() : "";
                        };
                    }

                };

                LiveFunc.fn_template_set($("#tp_snb_right_bidlist"), data, $("#divBidHstList"));
            },
            timeout: 1000
        });
    },

    fn_video_load: function () {

        //console.log("fn_video_load");

        isVideoConnect = true;
        var count = 0;

        //            if (LiveObj.videoType === 0 || LiveObj.videoType === 1) {
        //        try {

        //            var func = {
        //                init: function () { LiveFunc.fn_template_set($("#tp_player_flash"), {}, $(".video")); }
        //                ,
        //                play: function () {
        //                    if (!("MediaSource" in window && "WebSocket" in window))
        //                        document.getElementById("RtcPlayer").innerHTML = "브라우저가 지원하지 않습니다. 크롬브라우저를 설치해주십시요";

        //                    //RunPlayer("RtcPlayer", "588", "320", "115.68.235.77", 5119, false, "kalive", "", true, true, -1, "", false);
        //                    RunPlayer("RtcPlayer", "588", "320", "stream.camtour.net", 448, true, "kalive", "", true, true, -1, "", false);

        //                    var videoCatch = setInterval(function () {
        //                        try {
        //                            var $RtcPlayer_Video = document.getElementById("RtcPlayer_Video");
        //                            //$RtcPlayer_Video.muted = true;
        //                            var promise = $RtcPlayer_Video.play();
        //                            if (promise !== undefined) {
        //                                promise.then(function () {
        //                                    clearInterval(videoCatch);
        //                                }).catch(function (error) {
        //                                    console.log(error);
        //                                });
        //                            }
        //                        }
        //                        finally {
        //                            promise = null;
        //                            if (count > 30) clearInterval(videoCatch);
        //                            count++;
        //                        }
        //                    }, 200);
        //                }
        //                ,
        //                interval: function () {
        //                    clearInterval(playDelay);
        //                    playDelay = setInterval(function () {
        //                        var video = document.getElementById("RtcPlayer_Video");
        //                        video.currentTime = video.duration;
        //                    }, 60000);
        //                }
        //            }

        //            new Promise(func.init).then(new Promise(func.play)).then(new Promise(func.interval));
        //        }
        //        finally {
        //            func = null;
        //        }
        //    } else {
        //        LiveFunc.fn_template_set($("#tp_player_message"), { messageHtml: " 현재 브라우저에서 지원 되지 않는 서비스 입니다." }, $(".video"));
        //}






         if (LiveObj.videoType === 0 || LiveObj.videoType === 1) {
             try {

                 var func = {
                     init: function () { LiveFunc.fn_template_set($("#tp_player_flash"), {}, $(".video")); }
                     ,
                     play: function () {
                         if (!("MediaSource" in window && "WebSocket" in window))
                             document.getElementById("RtcPlayer").innerHTML = "브라우저가 지원하지 않습니다. 크롬브라우저를 설치해주십시요";

                         //RunPlayer("RtcPlayer", "588", "320", "115.68.235.77", 5119, false, "kalive", "", true, true, -1, "", false);
                         RunPlayer("RtcPlayer", "588", "320", "stream.camtour.net", 448, true, "kalive", "", true, true, -1, "", false);

                         var videoCatch = setInterval(function () {
                             try {
                                 var $RtcPlayer_Video = document.getElementById("RtcPlayer_Video");
                                 //$RtcPlayer_Video.muted = true;
                                 var promise = $RtcPlayer_Video.play();
                                 if (promise !== undefined) {
                                     promise.then(function () {
                                         clearInterval(videoCatch);
                                     }).catch(function (error) {
                                         console.log(error);
                                     });
                                 }
                             }
                             finally {
                                 promise = null;
                                 if (count > 30) clearInterval(videoCatch);
                                 count++;
                             }
                         }, 200);
                     }
                     ,
                     interval: function () {                        
                         clearInterval(playDelay);
                         playDelay = setInterval(function () {
                             var video = document.getElementById("RtcPlayer_Video");
                             video.currentTime = video.duration;
                         }, 60000);
                     }
                 }

                 new Promise(func.init).then(new Promise(func.play)).then(new Promise(func.interval));
             }
             finally {
                 func = null;
             }
         } else if (LiveObj.videoType === 2) {

             try {
                 var func = {
                     init: function () { LiveFunc.fn_template_set($("#tp_player_webrtc"), {}, $(".video")); }
                     ,
                     play: function () {
                         //webrtcPlayer = new UnrealWebRTCPlayer("RtcPlayer", "kaopus", "", "115.68.235.77", "5119", false, true, "tcp");
                         webrtcPlayer = new UnrealWebRTCPlayer("RtcPlayer", "kaopus", "", "stream.camtour.net", "448", true, true, "tcp");                        
                         webrtcPlayer.Play();

                         var videoCatch = setInterval(function () {
                             try {
                                 var $RtcPlayer_Video = document.getElementById("RtcPlayer");
                                 //$RtcPlayer_Video.muted = true;
                                 //webrtcPlayer.Play();
                                 var promise = $RtcPlayer_Video.play();
                                 if (promise !== undefined) {
                                     promise.then(function () {
                                         clearInterval(videoCatch);
                                     }).catch(function (error) {
                                         console.log(error);
                                     });
                                 }
                             }
                             finally {
                                 promise = null;
                                 if (count > 30) clearInterval(videoCatch);
                                 count++;
                             }
                         }, 200);
                     }
                 }

                 new Promise(func.init).then(new Promise(func.play));
             }
             finally {
                 func = null;
             }
         } else {
             LiveFunc.fn_template_set($("#tp_player_message"), { messageHtml: " 현재 브라우저에서 지원 되지 않는 서비스 입니다." }, $(".video"));
         }
    },
    fn_lazyload: function (t) {
        
        var $div_snb_left = (event && event.type === "scroll") ? event.currentTarget : LiveObj.$div_snb_left.get(0)
            , lazyloadImages = $div_snb_left.querySelectorAll("img.lazy")
            , curr_t_seq = $("#div_snb_left_tabs li.active > a").data("t_seq")
            , curr_t_lots = $div_snb_left.querySelectorAll("div[id=t_seq_" + curr_t_seq + "]>ul>li")
            , pageinfo = AuctionData.Table1.filter(function (e) { return e.t_seq == curr_t_seq })[0];
            ;

        function getAbsoluteTop(element) {
            return window.pageYOffset + element.getBoundingClientRect().top;
        }

        var p = getAbsoluteTop($div_snb_left);
        //console.log("부모절대좌표", $div_snb_left.find("div:visible").get(0), p);

        var arrLength = lazyloadImages.length;
        for (var i = 0; i < arrLength; i++) {
            var img = lazyloadImages[i];

            //자식 절대좌표
            var c = getAbsoluteTop(img);
            //console.log("자식절대좌표", c);

            var r = c - p;
            //console.log("상대좌표", r);

            if (r >= 0 && r <= $div_snb_left.offsetHeight) {
                img.src = img.dataset.src;
                img.classList.remove("lazy");
            }
        }


        var scrollTop = $div_snb_left.scrollTop;
        var innerHeight = $div_snb_left.offsetHeight;
        var scrollHeight = $div_snb_left.scrollHeight;

        //console.log(scrollTop, innerHeight, scrollHeight, curr_t_lots.length, curr_t_seq, AuctionData.Table1)

        if (scrollTop > 1 && (scrollTop + innerHeight >= scrollHeight - 30)) {
            //console.log("page down");

            new Promise(LiveFunc.fn_snb_left_conts_paging).then(new Promise(function () {
                $div_snb_left.scrollTop = scrollTop - 1;
            }))

            LiveFunc.fn_snb_left_conts_paging();
        } else if (scrollTop === 0) {

            //console.log("page up");            
            var isUp = innerHeight < scrollHeight;
            if (!isUp)
                isUp = pageinfo.page_no > 1 && curr_t_lots.length > 0 && curr_t_lots.length <= lot_page_size;
            
            //console.log("isUp", isUp);

            if (isUp) {
                new Promise(LiveFunc.fn_snb_left_conts_paging_up).then(new Promise(function () {
                    if (innerHeight === scrollHeight)
                        $div_snb_left.scrollTop = innerHeight - 1;
                    else {

                        $div_snb_left.scrollTop = 20;
                    }
                        
                }))
            } else {
                new Promise(LiveFunc.fn_snb_left_conts_paging).then(function () {
                    $div_snb_left.scrollTop = innerHeight - 1;
                })
            }


        }

        


    /*현재탭 이미지만 가져오자*/
        //var $div_snb_left = LiveObj.$div_snb_left
        //    , lazyloadImages = $div_snb_left.get(0).querySelectorAll("img.lazy")
        //    , lazyloadThrottleTimeout
        //    , curr_t_seq = $("#div_snb_left_tabs li.active > a").data("t_seq")
        //    , curr_t_lots = $div_snb_left.get(0).querySelectorAll("div[id=t_seq_" + curr_t_seq + "]>ul>li")
        //    ;





        ////console.time("fn_lazyload");

        ////console.log("fn_lazyload", (event) ? event.currentTarget : "");

        

        //function lazyload() {

        //    if (lazyloadThrottleTimeout) {
        //        clearTimeout(lazyloadThrottleTimeout);
        //    }

        //    lazyloadThrottleTimeout = setTimeout(function () {                

        //        function getAbsoluteTop(element) {
        //            return window.pageYOffset + element.getBoundingClientRect().top;
        //        }

        //        var p = getAbsoluteTop($div_snb_left.get(0));                
        //        //console.log("부모절대좌표", $div_snb_left.find("div:visible").get(0), p);

        //        var arrLength = lazyloadImages.length;
        //        for (var i = 0; i < arrLength; i++){
        //            var img = lazyloadImages[i];

        //            //자식 절대좌표
        //            var c = getAbsoluteTop(img);
        //            //console.log("자식절대좌표", c);

        //            var r = c - p;
        //            //console.log("상대좌표", r);
                    
        //            if (r >= 0 && r <= $div_snb_left.height()) {
        //                img.src = img.dataset.src;
        //                img.classList.remove("lazy");
        //            }
        //        }

        //        if (lazyloadImages.length === 0) {                    
        //            $div_snb_left.get(0).removeEventListener("scroll", lazyload);
        //            window.removeEventListener("resize", lazyload);
        //            window.removeEventListener("orientationChange", lazyload);
        //        }

        //        var scrollTop = $div_snb_left.scrollTop();
        //        var innerHeight = $div_snb_left.innerHeight();
        //        var scrollHeight = $div_snb_left.prop('scrollHeight');

        //        console.log(scrollTop, innerHeight, scrollHeight, curr_t_lots.length, curr_t_seq, AuctionData.Table1)
                
        //        if (scrollTop > 1 && (scrollTop + innerHeight === scrollHeight)) {       

        //            $div_snb_left.scrollTop(scrollTop - 1);

        //            console.log("page down 1");
        //            new Promise(LiveFunc.fn_snb_left_conts_paging)
        //                .then(new Promise(function () { $div_snb_left.scrollTop(scrollTop - 1); }))
        //                .then(new Promise(LiveFunc.fn_lazyload));
        //        } else if (scrollTop === 0) {                    
                    
        //            var isUp = innerHeight < scrollHeight;
        //            isUp = !isUp ? curr_t_lots.length > 0 : isUp
        //            console.log("isUp", isUp);

        //            if (isUp) {
        //                //new Promise(LiveFunc.fn_snb_left_conts_paging_up)
        //                //    .then(new Promise(function () {
        //                //        if (innerHeight === scrollHeight) {                                    
        //                //            $div_snb_left.scrollTop(innerHeight - 1);
        //                //        } else {                                    
        //                //            $div_snb_left.scrollTop(1);
        //                //        }
        //                //    }))
        //                //    .then(new Promise(LiveFunc.fn_lazyload));

        //                if (innerHeight === scrollHeight) {
        //                    $div_snb_left.scrollTop(innerHeight - 1);
        //                } else {
        //                    $div_snb_left.scrollTop(1);
        //                }

        //            } else {
        //                console.log("page down 2");

        //                //new Promise(LiveFunc.fn_snb_left_conts_paging)
        //                //    .then(new Promise(function () { $div_snb_left.scrollTop(1); }))
        //                //    .then(new Promise(LiveFunc.fn_lazyload));

        //                $div_snb_left.scrollTop(1);
        //            }

        //        }
        //        //else {                    
        //        //    console.log("넌어디", scrollTop, innerHeight, scrollHeight);
        //        //}
        //    }, 100);
        //}

        ////$div_snb_left.get(0).addEventListener("scroll", lazyload);
        ////window.addEventListener("resize", lazyload);
        ////window.addEventListener("orientationChange", lazyload);

        //if (t) {       
        //    lazyload();
        //}


        ////console.timeEnd("fn_lazyload");
    },

    fn_notice_slide: function () {

        noticeIdx++;

        var img_h = LiveObj.$notice_list.find("ul li"), img_n = LiveObj.$notice_list.find("ul li").length;
        if (noticeIdx === img_n) {
            noticeIdx = 0;
        }

        if (img_n > 1) {
            $(".notice ul").animate({ top: -img_h }, 900, "easeOutCubic", function () {
                $(".notice ul li:first").appendTo(".notice ul");
                $(".notice ul").css({ top: 0 });
            });
        }
    },
    fn_notice_list_pop: function () {
        $.ajax({
            type: "post",
            async: false,
            url: LiveApiHost + "/usp_Live_Auc_Notice_Info_Select",
            datatype: "json",
            contentType: "application/json;charset=UTF-8",
            data: JSON.stringify(Param),
            success: function (r) {
                try {
                    var notices = [];
                    notices.push("<p><ul>");
                    r.data.Table.forEach(function (row, idx) {
                        notices.push("<li style='list-style-type : disc;'>".concat(row.noti_memo, "</li>"));
                        notices.push("<li style='list-style-type : none;'>&nbsp;</li>");
                    })
                    notices.push("</ul></p>");

                    var opt = { titleHtml: "<h4><strong>NOTICE</strong></h4></br>", okText: Btn.OK };
                    opt.cancelText = "";
                    opt.messageHtml = notices.join("");
                    opt.completed = function (r) {
                        return r;
                    }
                    $.layerConfirm(opt);
                }
                finally {
                    opt = null;
                }
            },
            timeout: 2000
        });

    }
}

var AuctionData = JSON.parse(document.getElementById("jsonAuctionInfo").textContent)
    , Param = LiveFunc.fn_param()
    , isVideoConnect = false
    , keyDelay = null
    , playDelay = null
    , ticks = 0
    , clickOrTouch = "click"

    , noticeDelay = null
    , noticeSlideDelay = null
    , noticeIdx = 0

    , selected_call_cnt = 0
    , lot_page_size = 20
    ;

//var $midcontent = document.querySelector("#mid-content");
//$midcontent.addEventListener(clickOrTouch, function (e) {

//    if (LiveFunc.fn_hasClass(e.target, "like")) {
//        LiveFunc.fn_wish_insert();
//    }
//    else if (LiveFunc.fn_hasClass(e.target, "lotinfoforselect")) {        
//        LiveFunc.fn_selected_lotinfo();
//    }
//    else if (e.target.id === "curr_lot_num") {        
//        LiveFunc.fn_selected_lotinfo();
//    }
//});

$("body")
    .on("change", "#lot_num", LiveFunc.fn_selected_lotinfo)
    .on(clickOrTouch, "#btnBidding", LiveFunc.fn_bidding)
    .on(clickOrTouch, "#auc_bidhst_more", LiveFunc.fn_bidhst_list)
    .on(clickOrTouch, ".close, .modal", function (e) {
        e.preventDefault();
        e.stopPropagation();
        $(".modal").css("display", "none");
    })
    .on(clickOrTouch, ".details-cont", function (e) {
        /* 상세클릭으로 닫기 방지용 */
        e.preventDefault();
        e.stopPropagation();
    })
    .on(clickOrTouch, "li.krw>a", function (e) {
        $(".krw .op").toggleClass("bt");
        $(".krw").toggleClass("open");
    })
    .on(clickOrTouch, "li.krw>ul>li>a", function (e) {
        e.preventDefault();
        e.stopPropagation();

        var $this = this;

        var opt = { titleHtml: "", okText: Btn.OK, cancelText: Btn.CANCEL };

        opt.messageHtml = "<p class=\"cont1\">" + EtcMsg.Mdl_Currency_Terms_Msg1 + "</p>";
        opt.messageHtml += "<p class=\"content\">" + EtcMsg.Mdl_Currency_Terms_Msg2 + "</p>";
        opt.messageHtml += "<p class=\"agree\">" + EtcMsg.Mdl_Currency_Terms_Msg3 + "<input type=\"checkbox\" value=\"1\" style=\"width:15px;height:15px;cursor:pointer;\" id=\"chkCurrencyAgree\"></p>";
        opt.confirmed = function () {
            var $chkCurrencyAgree = $("#chkCurrencyAgree");
            if (!$chkCurrencyAgree.prop("checked"))
                alert(EtcMsg.Mdl_Currency_Terms_CheckBodx_Alert);

            return $chkCurrencyAgree.prop("checked");
        };
        opt.completed = function (r) {
            if (r) {
                $this.parentNode.parentNode.parentNode.querySelector(".op>span").textContent = $this.textContent;
                $this.parentNode.parentNode.parentNode.querySelector(".close>span").textContent = $this.textContent;
                $(".krw .op").toggleClass("bt");
                $(".krw").toggleClass("open");

                localStorage.setItem("CURRENCY", $this.textContent);
                LiveObj.$lot_num.trigger("change");
            }

            return r;
        };
        $.layerConfirm(opt);
    })
    .on(clickOrTouch, "li.kor>a", function (e) {
        $(".kor .op").toggleClass("bt");
        $(".kor").toggleClass("open");
    })
    .on(clickOrTouch, "li.kor>ul>li>a", function (e) {
        this.parentNode.parentNode.parentNode.querySelector(".op>span").textContent = this.textContent;
        this.parentNode.parentNode.parentNode.querySelector(".close>span").textContent = this.textContent;
        $(".kor .op").toggleClass("bt");
        $(".kor").toggleClass("open");

        var frm = document.createElement("form");
        frm.method = "GET";
        frm.name = "frmlangchg";
        frm.action = "/Home/SetLanguage";

        var hid_culture = document.createElement("input");
        hid_culture.setAttribute("type", "hidden");
        hid_culture.setAttribute("name", "culture");
        hid_culture.setAttribute("value", this.textContent);
        frm.appendChild(hid_culture);

        document.body.appendChild(frm);
        frm.submit();
    })
    .on(clickOrTouch, "#logout", function (e) {
        e.preventDefault();
        e.stopPropagation();

        var opt = { titleHtml: "", okText: Btn.OK, cancelText: Btn.CANCEL };

        opt.messageHtml = "<p class=\"cont1\">" + EtcMsg.Mdl_Logout + "</p>";
        opt.completed = function (r) {
            if (r) {
                self.close();
                document.location.href = EtcMsg.Logout_href;
                alert(EtcMsg.Mdl_Logout_End);
            }
        };
        $.layerConfirm(opt);
    })
    .on(clickOrTouch, "#login", function (e) {
        e.preventDefault();
        e.stopPropagation();

        document.location.href = "/Member/Login?returnUrl=" + location.pathname;
    })
    .on(clickOrTouch, "#video_onoff", function (e) {
        e.preventDefault();
        e.stopPropagation();

        var vedio = $(".video");
        if (vedio.is(":visible")) {
            vedio.hide();
            $(".mid .mid-bottom .midbt-content > div").css({ height: "577px" });
        } else {
            vedio.show();
            LiveFunc.fn_video_load();
            $(".mid .mid-bottom .midbt-content > div").css({ height: "247px" });
        }
    })
    .on(clickOrTouch, "#div_snb_left_tabs>ul>li", function (e) {
        
        LiveObj.$div_snb_left_tabs.find("ul>li").removeClass("active");

        var $this = $(e.currentTarget);
        $this.addClass("active");
        LiveObj.$div_snb_left.find("div.view").css("display", "none").eq($this.index()).css("display", "block");        
        LiveFunc.fn_lazyload();
    })
    .on(clickOrTouch, "#notice_list>ul>li", LiveFunc.fn_notice_list_pop)
    ;

LiveObj.$div_snb_left.on("scroll", LiveFunc.fn_lazyload);


toastr.options = {
    "closeButton": false,
    "debug": false,
    "newestOnTop": false,
    "progressBar": false,
    "positionClass": "toast-bottom-center",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}

$(document).ready(LiveFunc.fn_init);

