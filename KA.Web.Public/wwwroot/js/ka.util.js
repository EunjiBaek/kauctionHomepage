String.prototype.string = function (len) {
    var s = "",
        i = 0;
    while (i++ < len) {
        s += this;
    }
    return s;
};
String.prototype.zf = function (len) {
    return "0".string(len - this.length) + this;
};
Number.prototype.zf = function (len) {
    return this.toString().zf(len);
};

(function ($) {
    "use strict";

    ($.commonUtils = {
        decodeHTML: function (value) {
            return $("<div/>").html(value).text();
        },

        /* 페이지로딩 */
        loadingAni: function (target) {
            const $loding = document.createElement("div");
            $loding.id = "page_loading";
            $loding.innerHTML = `
            <svg version="1.1" id="loader-1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" width="60px" height="60px" viewBox="0 0 50 50" style="enable-background:new 0 0 50 50;" xml:space="preserve">
                <path fill="#151a43" d="M43.935,25.145c0-10.318-8.364-18.683-18.683-18.683c-10.318,0-18.683,8.365-18.683,18.683h4.068c0-8.071,6.543-14.615,14.615-14.615c8.072,0,14.615,6.543,14.615,14.615H43.935z">
                <animateTransform attributeType="xml" attributeName="transform" type="rotate" from="0 25 25" to="360 25 25" dur="0.6s" repeatCount="indefinite"></animateTransform>
                </path>
            </svg>
            `;
            target.appendChild($loding);
        },

        /* 툴팁생성 */
        tooltipFunc: function (elem) {
            if (elem) {
                const $tooltip = document.createElement("div");
                $tooltip.classList.add("tooltip_wrap");
                $tooltip.style.display = "none";
                elem.style.position = "relative";
                elem.appendChild($tooltip);
            }
        },
        /*------------------------------------------------------------------
            * @function:    notify
            * @param:       value (메시지)
            * @description: 메시지의 내용을 notify 처리 (중안 하단 셋팅)
            ------------------------------------------------------------------*/
        notify: function (value) {
            $.notify(
                {
                    message: value,
                },
                {
                    placement: {
                        from: "bottom",
                        align: "center",
                    },
                    animate: {
                        enter: "animated fadeInDown",
                        exit: "animated fadeOutUp",
                    },
                    delay: 2000,
                    z_index: 90000,
                }
            );
        },

        modalAuctionOpen: function (target) {
            if (document.querySelector(target)) {
                document.querySelector(target).classList.add("block");
                $.commonUtils.modalAuctionAddClass();
                document
                    .querySelector(".rightmodal-close")
                    .addEventListener("click", function () {
                        $.commonUtils.modalAuctionRemoveClass(target);
                    });
            }
        },

        modalAuctionAddClass: function () {
            document.querySelector("body").classList.add("yhidden");
            document.querySelector(".popupbg").classList.add("block");
        },

        modalAuctionRemoveClass: function (target) {
            if (document.querySelector(target)) {
                document.querySelector(target).classList.remove("block");
            }
            document.querySelector("body").classList.remove("yhidden");
            document.querySelector(".popupbg").classList.remove("block");
        },

        modalOpen: function (value, type, btn) {
            if (btn === "joinalert") {
                document.querySelector(
                    ".modal.fade .modal-content > div > .modal-btn > a.cancel"
                ).style.display = "block";
            }

            if (typeof type === "undefined") type = "";

            var elType = $("#modal-type");
            elType.attr("class", "modal-icon animate");
            elType.empty();
            if (type === "success" || type === "00") {
                $("#img-warning").hide();
                $("#img-safety").show();
            } else if (
                type === "requestBook" ||
                type === "requestBookComplete" ||
                type === "certificate"
            ) {
                $("#img-safety").hide();
                $("#img-warning").hide();
            } else {
                $("#img-safety").hide();
                $("#img-warning").show();
            }
            elType.append('<div class="modal-placeholder"></div>');
            elType.append('<div class="modal-fix"></div>');

            var el = $("#modal-common");
            el.find("#modal-message").html(value);
            el.find(".btn-ok").attr("onclick", "");
            el.find(".modal-btn").css({ display: "flex", "align-items": "center" });

            if (type === "requestBook") {
                let applyBook_tit = document.createElement("div");
                applyBook_tit.className = "applyBook_tit";
                applyBook_tit.innerHTML = value;
                el.find(".modal-content > div > h2").hide();
                el.find(".modal-content > div > .modal-cnt").hide();
                el.find(".modal-content > div > .modal-btn").hide();
                el.find(".modal-content > div").append(applyBook_tit);
                el.find(".modal-content .modal-close").css({
                    top: "19px",
                    right: "19px",
                });
                el.find("#applybook-form").show();
                el.find(".applybook-form").hide();
            } else if (type === "requestBookComplete") {
                let applyBook_tit = document.createElement("div");
                applyBook_tit.className = "applyBook_tit";
                applyBook_tit.innerHTML = value;
                el.find(".modal-content > div > h2").hide();
                el.find(".modal-content > div > .modal-cnt").hide();
                el.find(".modal-content > div > .modal-btn").hide();
                el.find(".modal-content > div").append(applyBook_tit);
                el.find(".modal-content .modal-close").css({
                    top: "19px",
                    right: "19px",
                });
                el.find("#applybook-form").hide();
                el.find(".applyBook_content_complete").show();
                el.find("#btn-modal-common-close").attr(
                    "onclick",
                    "$('#modal-common').modal('hide');"
                );

                if (el.find(".applybook-request-date").length < 1) {
                    el.find(".applybook-info-tel")
                        .removeClass("m-b-10")
                        .addClass("m-b-5");
                    el.find("#applyBookCompleteContent").append(
                        `<div class='m-l-10 m-b-10 applybook-request-date'>${ka.msg.mypage.requestDate}:
                            ${btn}
                        </div>`
                    );
                }
            } else if (type === "certificate") {
                let applyBook_tit = document.createElement("div");
                applyBook_tit.className = "applyBook_tit";
                applyBook_tit.innerHTML = value;
                el.find(".modal-content > div > h2").hide();
                el.find(".modal-content > div > .modal-cnt").hide();
                el.find(".modal-content > div > .modal-btn").hide();
                el.find(".modal-content > div").append(applyBook_tit);
                el.find(".modal-content .modal-close").css({
                    top: "19px",
                    right: "19px",
                });
                el.find("#certificate-form").show();
            }
            el.modal("show");
        },

        /*------------------------------------------------------------------
            2월14일
            * @function:    allAgreeCheck
            * @param:       check(input) allCheck(input)
            ------------------------------------------------------------------*/
        allAgreeCheck: function (check, allCheck) {
            let $check = check,
                $allCheck = allCheck;

            $check.forEach(function (el) {
                el.addEventListener("click", function () {
                    for (var i = 0, len = $check.length; i < len; ++i) {
                        if (!$check[i].checked) {
                            $allCheck.checked = false;
                            return;
                        }
                    }
                    $allCheck.checked = true;
                });
            });

            $allCheck.addEventListener("change", function (e) {
                for (let i = 0; i < $check.length; i++) {
                    $check[i].checked = e.target.checked;
                }
            });
        },

        /*------------------------------------------------------------------
            3월2일
            * @function:    응찰내역 상세 팝업
            * @param:          
            ------------------------------------------------------------------*/
        mypageModal: function (target) {
            // 모달팝업 title mypage_modal_content
            let $mypageModal = $(".mypage_modal"),
                $modalHeader = $(".mypage_modal .mypage_modal_header"),
                $modalHeaderTit = target.attr("data-tit"),
                $modalValue = target.attr("data-value"),
                $modalContent = $(".mypage_modal .mypage_modal_content > .cnt"),
                $modalBtn = $(".mypage_modal .modal_table_btn_wrap > button");

            $mypageModal.addClass("show");
            $("body").addClass("scroll_lock");

            // 모달팝업 타이틀변경
            $modalHeader.text($modalHeaderTit);
            $modalBtn.css("display", "block");

            if ($modalValue === "certified_number") {
                $modalBtn.text(ka.msg.common.submit2);
            } else if ($modalValue === "email_certify") {
                $modalBtn.text(ka.msg.common.cancel);
            } else {
                $modalBtn.text(ka.msg.common.confirm);
            }

            switch ($modalValue) {
                case "bid_detail":
                    $modalContent.children().hide();
                    $modalContent.find(".bid_detail_table").show();
                    break;
                case "pay_ship_details":
                    $modalContent.children().hide();
                    $modalContent.find(".pay_ship_details").show();
                    break;
                case "delivery_details":
                    $modalContent.children().hide();
                    $modalHeader.append(`<select class="default_select">
                        <option value="0">배송 받기</option>
                        <option value="1">방문 수령하기</option>
                    </select>`);
                    $(".mypage_modal_header > .default_select").selectMenuEvent(
                        $(".mypage_modal_content > .cnt > div.delivery_details")
                    );
                    $modalContent.find(".delivery_details").show();
                    break;
                case "password_reset":
                    $modalContent.children().hide();
                    $modalContent.find(".password_reset").show();
                    $modalBtn.text(ka.msg.common.submit);
                    $modalBtn.on("click", function () {
                        $.mypageUtils.changePassword();
                    });
                    break;
                case "certified_choice":
                    $modalContent.children().hide();
                    $modalContent.find(".certified_choice").show();
                    $modalBtn.css("display", "none");
                    break;
                case "certified_number":
                case "email_certify": // 2022.05.16 :: [계획] 해외 회원가입유형 서브 인증수단 이메일인증 추가 (#664) - 이메일 인증 팝업
                    $modalContent.children().hide();
                    $modalContent.find(".certified_number").show();
                    break;
                case "delete_account":
                    $modalContent.children().hide();
                    $modalContent.find(".delete_account").show();
                    $modalBtn.css("display", "none");
                    break;
                case "num_modify":
                    $modalContent.children().hide();
                    $modalContent.find(".num_modify").show();
                    break;
                case "preparation": // 2022.07.22 :: [계획] ISMS 국내 회원가입 본인인증 선택으로 변경+응찰필수조건처리 - 팝업
                    $modalContent.children().hide();
                    $modalContent.find(".preparation").show();
                    break;
                default:
                    $modalContent.empty();
            }
        },

        /*------------------------------------------------------------------
            4월27일
            * @function:    문자열 말줄임 표시
            * @param: leng(문자열 길이), target(타겟)         
            ------------------------------------------------------------------*/
        strSplit: function (leng, target) {
            let str = target.textContent;
            if (str.length > leng) {
                str = str.substr(0, leng - 2) + "...";
            }
            target.textContent = str;
        },

        /*------------------------------------------------------------------
            2월14일
            * @function:    tabmenu
            * @param:       selector(ul), content(탭메뉴 내용)
            ------------------------------------------------------------------*/
        tabMenu: function (selector, content) {
            let $tabMenu = selector,
                $menuItems = $tabMenu.find("li"),
                $selectMenuItem = $menuItems.eq(0),
                $content = $(`#${content}`);


            // 탭메뉴 내용 대입
            if ($content) {
                if ($menuItems.attr("data-menu")) {
                    $content.children().hide().eq($menuItems.attr("data-menu")).show();
                } else {
                    $content.children().hide().eq(0).show();
                }
            }

            $menuItems.on("click", function (e) {
                e.preventDefault();

                let idx = $(this).index();
                setSelectItem($(this));
                $setContent(idx);

            });

            let setSelectItem = function ($menuitem) {
                if ($selectMenuItem) {
                    $selectMenuItem.removeClass("active");
                }
                $selectMenuItem = $menuitem;
                $selectMenuItem.addClass("active");
            };

            let $setContent = function (idx) {
                $content.find("> div").hide().eq(idx).show();
            };
        },

        /*------------------------------------------------------------------
            * @function:    alert
            * @param:       value (메시지)
            *               type (success/error) - error 가 기본값
            * @description: 메시지의 내용을 모달 오픈
            ------------------------------------------------------------------*/
        alert: function (value, type, title) {
            if (typeof type === "undefined") type = "";

            var elType = $("#modal-type");
            elType.attr("class", "modal-icon animate");
            elType.empty();
            if (type === "success" || type === "00") {
                $("#img-warning").hide();
                $("#img-safety").show();
            } else {
                $("#img-safety").hide();
                $("#img-warning").show();
            }
            elType.append('<div class="modal-placeholder"></div>');
            elType.append('<div class="modal-fix"></div>');

            var el = $("#modal-common");
            el.find("#modal-message").html(value);
            el.find(".btn-ok").attr("onclick", "");
            if (typeof title === "string" && title !== "") {
                el.find("#modal-common-title").html(title);
            }
            el.modal("show");
        },

        /*------------------------------------------------------------------
            * @function:    alertWithFn
            * @param:       value (메시지)
            *               type (success/error) - error 가 기본값
            *               fn (확인 후 호출할 함수명)
            * @description: 메시지의 내용을 모달 오픈 후 fn에 정의된 함수 호출 처리
            ------------------------------------------------------------------*/
        alertWithFn: function (value, type, fn) {
            if (typeof type === "undefined") type = "";

            var elType = $("#modal-type");
            elType.attr("class", "modal-icon animate");
            elType.empty();
            if (type === "success" || type === "00") {
                //elType.addClass("modal-success");
                //elType.append('<span class="modal-line modal-tip animateSuccessTip"></span>');
                //elType.append('<span class="modal-line modal-long animateSuccessLong"></span>');
                $("#img-warning").hide();
                $("#img-safety").show();
            } else {
                //elType.addClass("modal-error");
                //elType.append('<span class="modal-x-mark">');
                //elType.append('<span class="modal-line modal-left animateXLeft"></span>');
                //elType.append('<span class="modal-line modal-right animateXRight"></span>');
                //elType.append('</span>');
                $("#img-safety").hide();
                $("#img-warning").show();
            }
            elType.append('<div class="modal-placeholder"></div>');
            elType.append('<div class="modal-fix"></div>');

            var el = $("#modal-common");
            el.find("#modal-message").html(value);
            el.find(".btn-ok").attr("onclick", fn);
            el.modal("show");
        },

        /*------------------------------------------------------------------
            * @function:    redirectAlert
            * @param:       value (메시지)
            *               type (success/error) - error 가 기본값
            *               page (확인 후 리다이렉트 할 URL)
            * @description: 메시지의 내용을 모달 오픈 후 page 로 이동 처리
            ------------------------------------------------------------------*/
        redirectAlert: function (value, type, page) {
            if (typeof type === "undefined") type = "";

            var elType = $("#modal-type");
            elType.attr("class", "modal-icon animate");
            elType.empty();
            if (type === "success" || type === "00") {
                //elType.addClass("modal-success");
                //elType.append('<span class="modal-line modal-tip animateSuccessTip"></span>');
                //elType.append('<span class="modal-line modal-long animateSuccessLong"></span>');
                $("#img-warning").hide();
                $("#img-safety").show();
            } else {
                //elType.addClass("modal-error");
                //elType.append('<span class="modal-x-mark">');
                //elType.append('<span class="modal-line modal-left animateXLeft"></span>');
                //elType.append('<span class="modal-line modal-right animateXRight"></span>');
                //elType.append('</span>');
                $("#img-safety").hide();
                $("#img-warning").show();
            }
            elType.append('<div class="modal-placeholder"></div>');
            elType.append('<div class="modal-fix"></div>');

            var el = $("#modal-common");
            el.find("#modal-message").html(value);
            el.find(".btn-ok").attr(
                "onclick",
                "window.open('" + page + "', '_self');"
            );
            el.modal("show");
        },

        openPopup: function (title, content) {
            if (title === "bid-info-1") {
                $("#modal-title").html("온라인 경매 호가표");
                $("#modal-body").html($("#" + title).html());
            } else if (title === "bid-info-2") {
                $("#modal-title").html("낙찰 수수료");
                $("#modal-body").html($("#" + title).html());
            } else if (title === "bid-info-3") {
                $("#modal-title").html("작품 픽업, 배송, 설치 및 보관");
                $("#modal-body").html($("#" + title).html());
            } else if (title === "popup-bid-list") {
                $("#modal-title").html("응찰현황");
                $("#modal-body").html($("#" + title).html());
            } else {
                $("#modal-title").html(title);
                $("#modal-body").html(content);
            }
            $("#modal-content").modal("show");
        },

        /*------------------------------------------------------------------
            * @function:    openLogin
            * @param:       msg (메시지)
            * @description: 로그인 팝업 오픈 처리 (msg 가 있는 경우 표기)
            ------------------------------------------------------------------*/
        openLogin: function (msg) {
            if (typeof msg === "string") {
                $("#validation-message").html(msg);
            } else {
                $("#validation-message").html("");
            }
            $("#modal-login").modal("show");
        },

        /*------------------------------------------------------------------
            * @function:    openWindow
            * @param:       url
            *               mode (전체모드)
            *               target
            * @description: 파라미터의 정보에 맞게 새창 오픈
            ------------------------------------------------------------------*/
        openWindow: function (url, mode, target) {
            var openWin;
            if (mode === "F") {
                openWin = window.open(
                    url,
                    target,
                    "toolbar=yes,scrollbars=yes,resizable=yes,height=" +
                    screen.height +
                    ",width=" +
                    screen.width +
                    ",fullscreen=yes"
                );
            } else {
                openWin = window.open(
                    url,
                    target,
                    "toolbar=yes,scrollbars=yes,resizable=yes,height=600,width=1024"
                );
            }
            if (window.focus) {
                openWin.focus();
            }
        },

        /*------------------------------------------------------------------
            * @function:    confirm
            * @param:       title (타이틀)
            *               message (메시지)
            *               func (확인 클릭 시 호출 함수 값)
            * @description: 메시지의 내용을 Confirm 오픈
            ------------------------------------------------------------------*/
        confirm: function (title, message, func) {
            const target = "modal-warning";
            const el = $(`#${target}`);
            el.modal("hide");
            el.find(`#${target}-title`).html(title);
            el.find(`#${target}-message`).html(message);
            el.modal("show");

            const confirmBtn = el.find('#btn-warning-confirm');
            confirmBtn.html(ka.msg.common.confirm);
            confirmBtn.attr(
                "onclick",
                `$('#${target}').click(); ${func}`
            );
        },

        confirmLogin: function (title, message, func) {
            var target = "modal-warning-login";
            var el = $("#" + target);
            el.modal("hide");
            el.find("#" + target + "-title").html(title);
            el.find("#" + target + "-message").html(message);
            el.modal("show");

            if (title === ka.msg.findIdPwd.findPwdTitle) {
                $("#btn-warning-login-close").html(ka.msg.common.close);
                $("#btn-warning-login-confirm").html(ka.msg.common.login);
            } else if (title === ka.msg.auction.Notice) {
                $("#btn-warning-login-close").html(ka.msg.main.changeNextTime);
                $("#btn-warning-login-confirm").html(ka.msg.main.changeNow);

                $("#btn-warning-login-close").attr(
                    "onclick",
                    "window.location.reload();"
                );
            }

            $("#btn-warning-login-confirm").attr(
                "onclick",
                "$('#" + target + "').click(); " + func
            );
        },

        confirmWithTarget: function (target, title, message, func) {
            var el = $("#modal-" + target);
            el.modal("hide");
            el.find("#" + target + "-title").html(title);
            el.find("#" + target + "-message").html(message);
            el.modal("show");

            $("#btn-" + target).attr("onclick", func);
        },

        confirmAuction: function (title, message, url) {
            var target = "modal-warning";
            var el = $("#" + target);
            el.modal("hide");
            el.find("#" + target + "-title").html(title);
            el.find("#" + target + "-message").html(message);
            el.find("#btn-warning-confirm").html(ka.msg.menu.goToAuctionIntroduction);
            $(".modal.fade .modal-content > div > .modal-section-btn > a").css(
                "width",
                "185px"
            );
            el.modal("show");
            $("#btn-warning-confirm").attr(
                "onclick",
                "window.open('" + url + "', '_self');"
            );
        },

        isNull: function (obj) {
            if (obj === "" || obj === null || typeof obj === "undefined") {
                return true;
            } else {
                return false;
            }
        },

        getFileExt: function (obj) {
            return obj.substring(obj.lastIndexOf(".") + 1, obj.length) || "";
        },

        /*------------------------------------------------------------------
            * @function:    getBirthFormat
            * @param:       value (생년월일 ex.19990101)
            * @description: 8자리 생년월일 포맷을 yyyy-MM-dd 포맷으로 변경하여 리턴
            ------------------------------------------------------------------*/
        getBirthFormat: function (value) {
            if (typeof value === "string" && value.length === 8) {
                return value.replace(/(\d{4})(\d{2})(\d{2})/, "$1-$2-$3");
            } else {
                return "";
            }
        },

        /*------------------------------------------------------------------
            * @function:    getDateTime
            * @param:       format (날짜 포맷 예.yyyy-MM-dd)
            *               addType (추가할 유형)
            *               addValue (추가 값)
            * @description: 현재 날짜를 format에 맞게 리턴하며, addType/addValue 에 따라 표기값 조정
            ------------------------------------------------------------------*/
        getDateTime: function (format, addType, addValue) {
            var h;
            var now = new Date();
            if (typeof addType === "string" && typeof addValue === "number") {
                if (addType === "s") {
                    now.setSeconds(now.getSeconds() + addValue);
                } else if (addType === "m") {
                    now.setMinutes(now.getMinutes() + addValue);
                }
            }

            return format.replace(/(yyyy|yy|MM|dd|E|hh|mm|ss|a\/p)/gi, function ($1) {
                switch ($1) {
                    case "yyyy":
                        return now.getFullYear();
                    case "yy":
                        return (now.getFullYear() % 1000).zf(2);
                    case "MM":
                        return (now.getMonth() + 1).zf(2);
                    case "dd":
                        return now.getDate().zf(2);
                    case "E":
                        return weekName[now.getDay()];
                    case "HH":
                        return now.getHours().zf(2);
                    case "hh":
                        return (now.getHours() % 12 ? now.getHours() % 12 : 12).zf(2);
                    case "mm":
                        return now.getMinutes().zf(2);
                    case "ss":
                        return now.getSeconds().zf(2);
                    case "a/p":
                        return now.getHours() < 12 ? "오전" : "오후";
                    default:
                        return $1;
                }
            });
        },

        /*------------------------------------------------------------------
            * @function:    setCookie
            * @param:       name (쿠키 name)
            *               value (쿠키 value)
                            expiredays (쿠키 만료일)
            * @description: 쿠키 설정
            ------------------------------------------------------------------*/
        setCookie: function (name, value, expiredays) {
            var date = new Date();
            date.setDate(date.getDate() + expiredays);
            document.cookie =
                name + "=" + escape(value) + "; expires=" + date.toUTCString();
        },

        setCookie00: function (name, value, expiredays) {
            var todayDate = new Date();
            var date = new Date(
                parseInt(todayDate.getTime() / 86400000) * 86400000 + 54000000
            );
            if (date > new Date()) {
                expiredays = expiredays - 1;
            }
            date.setDate(date.getDate() + expiredays);
            document.cookie =
                name +
                "=" +
                escape(value) +
                "; path=/; expires=" +
                date.toGMTString() +
                ";";
        },

        /*------------------------------------------------------------------
            * @function:    getCookie
            * @param:       name (쿠키 name)
            * @description: name 의 설정값 리턴
            ------------------------------------------------------------------*/
        getCookie: function (name) {
            var cookie = document.cookie;
            if (document.cookie != "") {
                var cookie_array = cookie.split("; ");
                for (var index in cookie_array) {
                    var cookie_name = cookie_array[index].split("=");
                    if (cookie_name[0] == name) {
                        return cookie_name[1];
                    }
                }
            }
            return "";
        },

        focus: function (target) {
            if (typeof target === "string") {
                var el = $("#" + target);
                el.focus();
                if (el.length > 0 && el.hasClass("form-control")) {
                    el.parents("div").addClass("focused");
                }
            }
        },

        sleep: function (ms) {
            // return new Promise((r) => setTimeout(r, ms));
            const wakeUpTime = Date.now() + ms;
            while (Date.now() < wakeUpTime) { }
        },

        print: function (title, body) {
            const objWin = window.open("", "print", "width=890, height=841");
            objWin.document.open();
            objWin.document.write(
                '<html><head><title>@Localizer["경매약관"] - @Localizer["메이저경매"]</title>'
            );
            objWin.document.write("</head><body>");
            objWin.document.write($("#content-body").html());
            objWin.document.write("</body></html>");
            objWin.focus();
            objWin.document.close();

            setTimeout(function () {
                objWin.print();
                objWin.close();
            }, 500);
        },
    }),
        ($.validateUtils = {
            opt: {
                memID: /^[a-z][a-z0-9]{4,12}$/i,
                memPwd: /([a-z]+[0-9]+|[0-9]+[a-z]+)/i,
                empty: /\s/,
                number: /^[0-9]*$/i,
            },

            regExp: function (obj, exp, msg) {
                if (typeof obj === "string") {
                    if (obj.search(exp) === -1) {
                        if (msg !== null && typeof msg !== "undefined")
                            $.commonUtils.alert(msg);
                        return false;
                    } else {
                        return true;
                    }
                } else {
                    if (obj.val().search(exp) === -1) {
                        obj.focus();
                        if (msg !== null && typeof msg !== "undefined")
                            $.commonUtils.alert(msg);
                        return false;
                    } else {
                        return true;
                    }
                }
            },

            maxLength: function (obj) {
                try {
                    if (obj.value.length > obj.maxLength) {
                        obj.value = obj.value.slice(0, obj.maxLength);
                    }
                } catch (e) { }
            },

            equals: function (x, y) {
                if (x === y) return true;

                if (!(x instanceof Object) || !(y instanceof Object)) return false;

                if (x.constructor !== y.constructor) return false;

                for (var p in x) {
                    if (!x.hasOwnProperty(p)) continue;

                    if (!y.hasOwnProperty(p)) return false;

                    if (x[p] === y[p]) continue;

                    if (typeof x[p] !== "object") return false;

                    if (!Object.equals(x[p], y[p])) return false;
                }

                for (p in y) {
                    if (y.hasOwnProperty(p) && !x.hasOwnProperty(p)) return false;
                }

                return true;
            },

            checkEmail: function (target) {
                var value = $("#" + target).val();
                var regExp =
                    /^[0-9a-zA-Z]([-_\.]?[0-9a-zA-Z])*@[0-9a-zA-Z]([-_\.]?[0-9a-zA-Z])*\.[a-zA-Z]{2,3}$/i;
                return regExp.test(value);
            },

            checkBusinessNum: function (value) {
                if (value !== "") {
                    var reqParam = {};
                    reqParam["business_num"] = value;
                    var result = $.ajaxUtils.getApiData(
                        "/api/Member/CheckBusinessNum",
                        reqParam,
                        null,
                        false
                    );
                    if ($.ajaxUtils.getResultCode(result) === "00") {
                        return result.message === "True" ? true : false;
                    } else {
                        return false;
                    }
                } else {
                    return false;
                }
            },

            isNullOrEmpty: function (target, msg) {
                let el = document.querySelector(target);
                if (el) {
                    let result = el.value.replace(/ /g, "") === "" ? true : false;
                    if (result) {
                        if (typeof msg === "string" && msg !== "") {
                            $.commonUtils.alert(msg);
                        }
                    }
                    return result;
                } else {
                    return true;
                }
            },
        }),
        ($.htmlUtils = {
            getCheckedValue: function (target, delimiter) {
                delimiter = delimiter ? delimiter : "|";
                let value = "";
                let el = document.querySelectorAll("[name=" + target + "]:checked");
                if (el.length > 0) {
                    el.forEach(function (item) {
                        value = value !== "" ? value + delimiter : value;
                        value += item.value;
                    });
                }
                return value;
            },
            setAttribute: function (target, name, value) {
                if (document.querySelector(target)) {
                    document.querySelector(target).setAttribute(name, value);
                }
            }
        }),
        ($.datetimeUtils = {
            /*------------------------------------------------------------------
              * @function:    format
              * @param:       value (시, 분, 초의 int형 값)
              * @description: 10보다 작은 경우 "0"을 붙여 2자리 값으로 리턴
              ------------------------------------------------------------------*/
            format: function (value) {
                return value < 10 ? "0" + value.toString() : value.toString();
            },

            today: function () {
                const today = new Date();
                return (
                    today.getFullYear() +
                    "-" +
                    $.datetimeUtils.format(today.getMonth() + 1) +
                    "-" +
                    $.datetimeUtils.format(today.getDate())
                );
            },

            /*------------------------------------------------------------------
              * @function:    dateDiff
              * @param:       orgDate (비교 날짜 개체)
              *               targetDate (비교 날짜 개체)
              * @description: 두개의 날짜를 비교하여 차이 일수를 리턴
              ------------------------------------------------------------------*/
            dateDiff: function (type, orgDate, targetDate) {
                var org = org instanceof Date ? orgDate : new Date(orgDate);
                var target =
                    targetDate instanceof Date ? targetDate : new Date(targetDate);

                org = new Date(
                    org.getFullYear(),
                    org.getMonth() + 1,
                    org.getDate(),
                    org.getHours(),
                    org.getMinutes(),
                    org.getSeconds()
                );
                target = new Date(
                    target.getFullYear(),
                    target.getMonth() + 1,
                    target.getDate(),
                    target.getHours(),
                    target.getMinutes(),
                    target.getSeconds()
                );

                var diff = target - org;
                var timeGap = new Date(0, 0, 0, 0, 0, 0, target - org);

                if (type === "D") {
                    return Math.floor(diff / (1000 * 3600 * 24));
                } else if (type === "H") {
                    return timeGap.getHours();
                } else if (type === "M") {
                    return timeGap.getMinutes();
                } else if (type === "S") {
                    return timeGap.getSeconds();
                } else {
                    return 0;
                }
            },

            toString: function (obj, format) {
                format = format ? format : "yyyy-MM-dd";
                const year = obj.getFullYear();
                const month = $.datetimeUtils.format(obj.getMonth() + 1);
                const date = $.datetimeUtils.format(obj.getDate());
                return format
                    .replace(/yyyy/g, year)
                    .replace(/MM/g, month)
                    .replace(/dd/g, date);
            },
        }),
        ($.stringUtils = {
            left: function (value, len) {
                if (len <= 0) return "";
                else if (len > String(value).length) return str;
                else return String(value).substring(0, length);
            },
            right: function (value, len) {
                if (len <= 0) return "";
                else if (len > String(value).length) return value;
                else {
                    var iLen = String(value).length;
                    return String(value).substring(iLen, iLen - len);
                }
            },
            comma: function (value) {
                return typeof value === "undefined"
                    ? ""
                    : value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            },
            jsonToQueryString: function (json) {
                try {
                    $.each(json, function (key, value) {
                        if (value === "" || value === null) {
                            delete json[key];
                        }
                        if (key === "page" && value.toString() === "1") {
                            delete json[key];
                        }
                    });
                    return (
                        "?" +
                        Object.keys(json)
                            .map(function (key) {
                                return encodeURI(key) + "=" + encodeURI(json[key]);
                            })
                            .join("&")
                    );
                } catch (e) {
                    return "";
                }
            },
            queryStringToJson: function (qs) {
                qs = qs || location.search.slice(1);

                if (qs === "") return null;

                var pairs = qs.split("&");
                var result = {};
                pairs.forEach(function (p) {
                    var pair = p.split("=");
                    var key = pair[0];
                    var value = decodeURI(pair[1] || "");

                    if (result[key]) {
                        if (
                            Object.prototype.toString.call(result[key]) === "[object Array]"
                        ) {
                            result[key].push(value);
                        } else {
                            result[key] = [result[key], value];
                        }
                    } else {
                        result[key] = value;
                    }
                });

                return JSON.parse(JSON.stringify(result));
            },
            numberToUnit: function (number) {
                var inputNumber = number < 0 ? false : number;
                var unitWords = ["", "만", "억"];
                var splitUnit = 10000;
                var splitCount = unitWords.length;
                var resultArray = [];
                var resultString = "";
                for (var i = 0; i < splitCount; i++) {
                    var unitResult =
                        (inputNumber % Math.pow(splitUnit, i + 1)) / Math.pow(splitUnit, i);
                    unitResult = Math.floor(unitResult);
                    if (unitResult > 0) {
                        resultArray[i] = unitResult;
                    }
                }
                for (var i = 0; i < resultArray.length; i++) {
                    if (!resultArray[i]) continue;
                    resultString = String(resultArray[i]) + unitWords[i] + resultString;
                }
                return resultString === "" ? "0" : resultString;
            },
        }),
        ($.listUtils = {
            language: {
                search:
                    "<div class='form-group-default input-group'>_INPUT_<div class='input-group-append'><span class='input-group-text black'><i class='fas fa-search fa-fw'></i></span></div></div>",
                searchPlaceholder: "Search...",
                info: "<b>_TOTAL_</b> 건",
                infoEmpty: "0 건",
                infoFiltered: "/ 총 <b>_MAX_</b> 건 중",
                emptyTable:
                    "<p class='p-t-50 p-b-50 hint-text'>등록된 게시물이 없습니다.</p>",
                zeroRecords:
                    "<p class='p-t-50 p-b-50 hint-text'>검색 결과가 없습니다.</p>",
                processing: "<p class='p-t-50 p-b-50 hint-text'>처리중...</p>",
                paginate: {
                    first: "<i class='far fa-chevron-double-left fa-fw fs-9'></i>",
                    previous: "<i class='far fa-chevron-left fa-fw fs-9'></i>",
                    next: "<i class='far fa-chevron-right fa-fw fs-9'></i>",
                    last: "<i class='far fa-chevron-double-right fa-fw fs-9'></i>",
                },
            },
        }),
        ($.ajaxUtils = {
            /*------------------------------------------------------------------
              * @function:    setParameterString
              * @description: 태그의 속성 중 param 이 Y인 요소의 값을 json 처리
              * @returns:     json (element id/value) 리턴
              ------------------------------------------------------------------*/
            setParameter: function (param) {
                var target = ["select", "input", "textarea"];
                $.each(target, function (rootIndex, rootItem) {
                    $.each($(rootItem), function (index, item) {
                        if (
                            item.attributes["param"] !== undefined &&
                            item.attributes["param"].value === "Y"
                        ) {
                            var targetID =
                                typeof item.attributes["param-id"] === "undefined"
                                    ? item.id.replace(/-/gi, "_")
                                    : item.attributes["param-id"].value.replace(/-/gi, "_");
                            if (targetID === "") return true;
                            if (
                                $("#" + item.id).val() !== "" &&
                                (item.type === "hidden" ||
                                    $("#" + item.id).css("display") !== "none")
                            ) {
                                switch (rootItem) {
                                    case "select":
                                        param[targetID] = $(
                                            "#" + item.id + " option:selected"
                                        ).val();
                                        break;
                                    case "input":
                                        switch (item.type) {
                                            case "radio":
                                                param[targetID] = $(
                                                    "input[name='" + item.name + "']:checked"
                                                ).val();
                                                break;
                                            default:
                                                param[targetID] = $("#" + item.id).val();
                                                break;
                                        }
                                        break;
                                    default:
                                        param[targetID] = $("#" + item.id).val();
                                        break;
                                }
                            } else {
                                param[targetID] = "";
                            }
                        }
                    });
                });
                return param;
            },

            /*------------------------------------------------------------------
              * @function:    getApiData
              * @description: ajax 호출 처리
              * @returns:     동기 호출인 경우 결과값 리턴
              ------------------------------------------------------------------*/
            getApiData: function (url, parameter, callback, async) {
                if (typeof parameter === "object")
                    parameter = JSON.stringify(parameter);
                if (typeof async !== "boolean") async = true;

                var result;

                $.ajax({
                    type: "post",
                    async: async,
                    url: url,
                    datatype: "json",
                    contentType: "application/json",
                    data: parameter,
                    beforeSend: function () {
                        // 로딩중 처리 시작
                    },
                    success: function (args) {
                        if (
                            typeof args === "string" &&
                            args.indexOf("<!DOCTYPE html>") > -1
                        ) {
                            alert("세션이 만료되었습니다.\n다시 로그인 하시기 바랍니다.");
                            window.location.href =
                                "/?ReturnUrl=" + encodeURI(window.location.pathname);
                        }
                        if (async && typeof callback === "function") {
                            callback.call(this, args, parameter);
                        }

                        if (!async) {
                            result = args;
                        }
                    },
                    error: function (args) { },
                    complete: function (args) {
                        // 로딩중 처리 종료
                    },
                });

                return result;
            },

            /*------------------------------------------------------------------
              * @function:    getResultCode
              * @description: ajax 처리 결과 json 에서 결과 코드 값 확인
              * @returns:     결과 코드 리턴
              ------------------------------------------------------------------*/
            getResultCode: function (result) {
                return typeof result !== "undefined" &&
                    typeof result.code !== "undefined"
                    ? result.code
                    : "";
            },

            /*------------------------------------------------------------------
              * @function:    getUploadData
              * @description: ajax 업로드 처리 결과 json 에서 결과 코드 값 확인
              * @returns:     업로드 결과 json 리턴
              ------------------------------------------------------------------*/
            getUploadData: function (fileID, uploadUrl) {
                var updateResult = {};
                var file = $("#" + fileID);
                if (file.val() !== "") {
                    var formData = new FormData();
                    formData.append("uploadfile", file.get(0).files[0]);
                    $.ajax({
                        url: uploadUrl,
                        async: false,
                        processData: false,
                        contentType: false,
                        data: formData,
                        type: "POST",
                        success: function (result) {
                            if (result !== null && result.file_names.length > 0) {
                                updateResult["result"] = true;

                                var fileInfo = {};
                                fileInfo["filename"] = result.file_names[0];

                                updateResult["file_info"] = fileInfo;
                                updateResult["etc"] = result.etc;
                            } else {
                                updateResult["result"] = false;
                            }
                        },
                        error: function () {
                            updateResult["result"] = false;
                        },
                    });
                }
                return updateResult;
            },

            /*------------------------------------------------------------------
              * @function:    getUploadDatas
              * @description: ajax 업로드 처리 결과 json 에서 결과 코드 값 확인
              * @returns:     업로드 결과 json 리턴
              ------------------------------------------------------------------*/
            getUploadDatas: function (files, uploadUrl) {
                var updateResult = {};
                if (files !== null) {
                    var formData = new FormData();
                    for (var i = 0; i < files.length; i++) {
                        formData.append("uploadfile", files[i]);
                    }
                    $.ajax({
                        url: uploadUrl,
                        async: false,
                        processData: false,
                        contentType: false,
                        data: formData,
                        type: "POST",
                        success: function (result) {
                            if (result !== null && result.file_names.length > 0) {
                                updateResult["result"] = true;

                                var fileInfo = {};
                                fileInfo["filename"] = result.file_names;

                                updateResult["file_info"] = fileInfo;
                                updateResult["etc"] = result.etc;
                            } else {
                                updateResult["result"] = false;
                            }
                        },
                        error: function () {
                            updateResult["result"] = false;
                        },
                    });
                }
                return updateResult;
            },

            callApiAndRedirect: function (url, parameter, redirectUrl, target) {
                if (typeof parameter === "object")
                    parameter = JSON.stringify(parameter);
                $.ajax({
                    type: "post",
                    async: false,
                    url: url,
                    datatype: "json",
                    contentType: "application/json",
                    data: parameter,
                    success: function () { },
                    error: function () { },
                });

                if (typeof redirectUrl === "string" && redirectUrl !== "") {
                    if (
                        isApp === "True" &&
                        (redirectUrl.toLowerCase().indexOf("http") < 0 ||
                            redirectUrl.indexOf(window.location.hostname) > 0)
                    ) {
                        window.location.href = redirectUrl;
                    } else {
                        try {
                            if (
                                JSON.parse(parameter).type === "I" ||
                                JSON.parse(parameter).type === "N"
                            ) {
                                window.open(
                                    redirectUrl,
                                    "kauction_" + JSON.parse(parameter).type,
                                    "width=980,height=650,location=no,scrollbars=yes,menubar=no,resizable=no,status=no,toolbar=no"
                                );
                            } else {
                                window.open(redirectUrl, target);
                            }
                        } catch (e) {
                            window.open(redirectUrl, target);
                        }
                    }
                }
            },
        }),
        ($.joinUtils = {
            joinMsg: "",
            joinType: "",
            reqType: "",
            authFlag: [false, false, false, false],
            field: ["id", "pwd", "pwd-re", "email", "mobile", "name", "birth-date"],
            corpField: [
                "company-reg-no",
                "company-name",
                "company-president",
                "company-rep-tel",
                "company-reg-doc",
                "company-business-card",
            ],
            fields: [
                [
                    "id",
                    "name",
                    "birth-date",
                    "sex",
                    "mobile",
                    "email",
                    "pwd",
                    "pwd2",
                    "country-code",
                    "zipcode",
                    "address",
                    "address2",
                ],
                [
                    "id",
                    "name",
                    "birth-date",
                    "sex",
                    "mobile",
                    "email",
                    "pwd",
                    "pwd2",
                    "country-code",
                    "zipcode",
                    "address",
                    "address2",
                    "identification",
                ],
                [
                    "id",
                    "name",
                    "birth-date",
                    "sex",
                    "mobile",
                    "email",
                    "pwd",
                    "pwd2",
                    "company-name",
                    "company-president",
                    "company-rep-tel",
                    "company-tel",
                    "company-reg-no",
                    "company-reg-doc",
                    "company-business-card",
                    "zipcode",
                    "address",
                    "address2",
                ],
                [
                    "id",
                    "name",
                    "birth-date",
                    "sex",
                    "mobile",
                    "email",
                    "pwd",
                    "pwd2",
                    "country-code",
                    "company-name",
                    "company-president",
                    "company-rep-tel",
                    "company-tel",
                    "company-reg-no",
                    "company-reg-doc",
                    "company-business-card",
                    "zipcode",
                    "address",
                    "address2",
                ],
            ],
            extraFields: [
                [
                    "job-code",
                    "company-name",
                    "company-tel",
                    "company-fax",
                    "company-zipcode",
                    "company-address",
                    "company-address2",
                    "delivery-zipcode",
                    "delivery-address",
                    "delivery-address2",
                ],
            ],
            element_wrap: null,
            memType: 0,
            authType: "",
            extraParam: null,
            authSequence: null,
            validateFlag: false,
            validateBusinessNum: false,

            /*------------------------------------------------------------------
                * @function:    checkJoinCondition                        
                * @description: 약관 체크 여부 확인
                ------------------------------------------------------------------*/
            checkJoinCondition: function () {
                let result = true;
                for (var i = 0; i < $("input[name=join-conditions]:checkbox").length; i++) {
                    if (result) {
                        let el = $("input[name=join-conditions]:checkbox");
                        if (el[i].dataset.requiredYn === "Y" && !el[i].checked) {
                            result = false;
                        }
                    }
                }
                return result;
            },

            /*------------------------------------------------------------------
                * @function:    openAuth
                * @param:       memType (국내개인:1,2,3,4)
                *               authType (M:핸드폰, C:신용카드)
                * @description: 태그의 속성 중 param 이 Y인 요소의 값을 json 처리
                ------------------------------------------------------------------*/
            openAuth: function (memType, authType, reqType, target) {
                $.joinUtils.memType = memType;
                $.joinUtils.authType = authType;
                $.joinUtils.reqType = typeof reqType === "string" && reqType !== "" ? reqType : "";

                if (memType === 1 || memType === 3) {
                    // 1. 국내개인고객, 3. 국내법인고객
                    // App Check
                    if (isMobile === "True") {
                        window.open(
                            "/Member/" +
                            (authType === "C" ? "AuthCardSend" : "AuthMobileSend") + "/00" + memType +
                            (reqType !== "" ? "/" + $.joinUtils.reqType : "") +
                            "?redirect=" + decodeURI(location.pathname) +
                            (typeof target === "string" ? "&target=" + target : ""),
                            "_self"
                        );
                    } else {
                        window.open(
                            "/Member/" + (authType === "C" ? "AuthCardSend" : "AuthMobileSend") + "/00" + memType +
                            (reqType !== "" ? "/" + $.joinUtils.reqType : "") +
                            "?redirect=" + decodeURI(location.pathname) +
                            (typeof target === "string" ? "&target=" + target : ""),
                            "AuthSend",
                            "width=430,height=640,scrollbar=yes"
                        );
                    }
                } else if (memType === 2 || memType === 4) {
                    if ($("#mobile-" + memType.toString()).val() === "") {
                        $.joinUtils.validateMsg(
                            "mobile-cont-" + memType.toString(),
                            ka.msg.join.memMobileRuleEmpty
                        );
                        return false;
                    } else {
                        if ($.joinUtils.setMobileForeignAuth("INSERT", reqType)) {
                            $("#mobile-cont-" + memType.toString() + "-error").remove();

                            $("#fg-mobile-auth-num-" + memType.toString())
                                .parent()
                                .removeClass("disabled");
                            $("#fg-mobile-auth-num-" + memType.toString())
                                .parent()
                                .find("span")
                                .removeClass("disabled");
                            $("#fg-mobile-" + memType.toString())
                                .parent()
                                .addClass("disabled");
                            $("#mobile-" + memType.toString()).attr("disabled", "disabled");
                            $("#confirm-code-" + memType.toString()).focus();
                            $("#confirm-code-" + memType.toString()).removeAttr("disabled");

                            //[Temp]
                            $("#auth-remain-time-" + memType.toString())
                                .countdown(
                                    $.commonUtils.getDateTime("yyyy/MM/dd HH:mm", "s", 1800)
                                )
                                .on("update.countdown", function (event) {
                                    $(this).html(event.strftime("%M:%S"));
                                })
                                .on("finish.countdown", function (event) {
                                    $(this).html("");
                                    $.joinUtils.openAuthCancel(memType);
                                });
                            $.joinUtils.authFlag[memType - 1] = true;
                        }
                    }
                }
            },

            /*------------------------------------------------------------------
              * @function:    openAuthV2
              * @param:       memType (국내개인:1,2,3,4)
              *               authType (M:핸드폰, C:신용카드)
              * @description: 태그의 속성 중 param 이 Y인 요소의 값을 json 처리
              ------------------------------------------------------------------*/
            openAuthV2: function (memType, authType) {                
                $.joinUtils.memType = memType === "000" ? $("#join-certified-select").val() : memType;
                $.joinUtils.authType = authType;

                var result = memType === "001" || memType === "003" ? $.joinUtils.setMobileAuth("INSERT", "J", authType) : $.joinUtils.setMobileForeignAuth("INSERT", "J", authType);
                if (result) {
                    if (authType === "M") {
                        $("#auth-remain-time-" + memType.toString())
                            .countdown(
                                $.commonUtils.getDateTime("yyyy/MM/dd HH:mm", "s", 1800)
                            )
                            .on("update.countdown", function (event) {
                                $(this).html(event.strftime("%M:%S"));
                            })
                            .on("finish.countdown", function (event) {
                                $(this).html("");
                                $.joinUtils.openAuthCancel(memType);
                            });
                    }
                    $.joinUtils.authFlag[memType - 1] = true;
                }

                //20220713
                //if (memType === "001" || memType === "003") {
                //    // 1. 국내개인고객, 3. 국내법인고객
                //    var authUrl =
                //        "/Member/" +
                //        (authType === "C" ? "AuthCardSend" : "AuthMobileSend") +
                //        "/" +
                //        memType +
                //        "/JoinStep";
                //    if (isMobile === "True") {
                //        window.open(authUrl, "_self");
                //    } else {
                //        window.open(
                //            authUrl,
                //            "AuthSend",
                //            "width=430,height=640,scrollbar=yes"
                //        );
                //    }
                //} else if (memType === "002" || memType === "004") {
                //    if ($.joinUtils.setMobileForeignAuth("INSERT", "J", authType)) {
                //        if (authType == "M") {
                //            $("#auth-remain-time-" + memType.toString())
                //                .countdown(
                //                    $.commonUtils.getDateTime("yyyy/MM/dd HH:mm", "s", 1800)
                //                )
                //                .on("update.countdown", function (event) {
                //                    $(this).html(event.strftime("%M:%S"));
                //                })
                //                .on("finish.countdown", function (event) {
                //                    $(this).html("");
                //                    $.joinUtils.openAuthCancel(memType);
                //                });
                //        }
                //        $.joinUtils.authFlag[memType - 1] = true;
                //    }
                //}
            },

            /*------------------------------------------------------------------
              * @function:    openAuthConfirm
              * @param:       memType (국내개인:1,2,3,4)
              *               authType (M:핸드폰, C:신용카드)
              * @description: 법인 가입시 휴대폰 인증 요청 클릭 시 Confirm 창 처리
              ------------------------------------------------------------------*/
            openAuthConfirm: function (memType, authType, reqType) {
                if ($("#mobile-" + memType.toString()).val().replace(/ /gi, "") === "") {
                    $.joinUtils.validateMsg("mobile-cont-" + memType.toString(), ka.msg.join.memMobileRuleEmpty);
                    return false;
                } else if ($("#mobile-" + memType.toString()).val().replace(/ /gi, "").indexOf("82") === 0) {
                    $.commonUtils.alert(ka.msg.join.memAuthNumber);
                } else {
                    $.joinUtils.validateMsg("mobile-cont-" + memType.toString(), "");
                    var paramType = typeof reqType === "string" && reqType !== "" ? reqType : "J";

                    var countryPhoneCode = $(".overseas-individuals-member").find($(".iti__selected-dial-code")).html();

                    $.commonUtils.confirm(
                        ka.msg.join.reqCode,
                        ka.msg.join.reqCodeConfirm.replace(/{MOBILE}/gi, (memType === "001" || memType === "003" ? "" : "(" + countryPhoneCode + ") ") + $("#mobile-" + memType.toString()).val()),
                        "$.joinUtils.openAuthV2('" + memType + "', '" + authType + "', '" + paramType + "');"
                    );
                }
            },

            openAuthConfirmEmail: function (memType, authType) {
                if ($("#email-auth-input").val().replace(/ /gi, "") === "") {
                    $.joinUtils.validateMsg("email-auth-input", ka.msg.join.memEmailRule);
                    return false;
                }
                var paramType =
                    typeof reqType === "string" && reqType !== "" ? reqType : "J";
                $.commonUtils.confirm(
                    ka.msg.join.reqCode,
                    ka.msg.join.reqCodeConfirmEmail.replace(
                        /{EMAIL}/gi,
                        $("#email-auth-input").val()
                    ),
                    "$.joinUtils.openAuthV2('" + memType + "', 'E', '" + paramType + "');"
                );
            },

            /*------------------------------------------------------------------
              * @function:    openAuthCancel
              * @param:       memType (국내개인:1,2,3,4)
              * @description: 법인 가입시 휴대폰 인증 요청 클릭 후 취소 처리
              ------------------------------------------------------------------*/
            openAuthCancel: function (memType) {
                memType = memType === "000" ? $("#join-certified-select").val() : memType;

                $("#auth-remain-time-" + memType.toString()).countdown("stop");
                $("#auth-remain-time-" + memType.toString()).html('');

                $("#mobile-" + memType.toString()).removeAttr("disabled");
                $("#fg-mobile-" + memType.toString())
                    .parent()
                    .removeClass("disabled");
                $("#fg-mobile-req-btn-" + memType.toString()).show();
                $("#fg-mobile-cancel-btn-" + memType.toString()).hide();

                $("#confirm-code-" + memType.toString()).attr("disabled", "disabled");
                $("#fg-mobile-auth-" + memType.toString()).addClass("disabled");
                $("#fg-mobile-auth-btn-" + memType.toString()).hide();
            },

            openAuthChange: function (memType) {
                $("#fg-mobile-change-btn-" + memType).hide();
                $("#fg-mobile-req-btn-" + memType).show();
                $("#fg-mobile-cont-" + memType).removeClass("disabled");
                $("#mobile-" + memType).removeAttr("disabled");
            },

            /*------------------------------------------------------------------
                * @function:    setMobileAuth
                * @description: 국내개인고객/국내법인고객 휴대전화 인증 요청 및 확인 처리
                ------------------------------------------------------------------*/
            setMobileAuth: function (mode, type, auth) {
                const util = $.joinUtils;
                if (mode === "CONFIRM") {
                    if (auth === "M" && $("#confirm-code-" + util.memType.toString()).val() === "") {
                        $.joinUtils.validateMsg("mobile-cont-" + util.memType.toString(), ka.msg.join.reqCodeEmpty);
                        return false;
                    }
                    if (auth === "E" && $("#email-auth-confirm").val() === "") {
                        $.joinUtils.validateMsg("email-auth-input", ka.msg.join.reqCodeEmpty);
                        return false;
                    }
                }

                var param = {};
                param["mode"] = mode;
                param["seq"] = util.authSequence;
                param["type"] = typeof type === "string" && type !== "" ? type : "J";
                param["type_detail"] = util.memType.toString().length > 1 ? util.memType.toString() : "00" + util.memType.toString();
                param["req_type"] = "JoinStep";
                param["auth"] = auth;

                // 국가코드 처리 추가
                if (auth === "E") {
                    param["mobile_no"] = $("#email-auth-input").val();
                    param["auth_no"] = $("#email-auth-confirm").val();
                } else {
                    var countryPhoneCode = $(".overseas-individuals-member").find($(".iti__selected-dial-code")).html();
                    // param["mobile_no"] = countryPhoneCode.replace(/\+/gi, "") + $("#mobile-" + util.memType.toString()).val();
                    param["mobile_no"] = $("#mobile-" + util.memType.toString()).val();
                    param["auth_no"] = $("#confirm-code-" + util.memType.toString()).val();
                }

                var result = $.ajaxUtils.getApiData("/api/Member/SetMobileAuth", param, null, false);
                if (result.code === "00") {
                    util.authSequence = result.data.Sequence;
                    if (mode === "INSERT") {
                        if (location.href.indexOf("www.dev.") > -1 || location.href.indexOf("www.release.") > -1 || location.href.indexOf("localhost") > -1) {
                            $.commonUtils.alert("테스트계는 인증번호를 제공 합니다.<br />인증번호 : " + result.data.AuthNo, "success");
                        }
                        if (auth === "E") {
                            $("#email-auth-input-error").css("color", "red");
                            $("#email-auth-input-error").html("");

                            $("#email-auth-input").attr("disabled", "disabled");
                            $("#btn-email-auth-request").attr("disabled", "disabled");
                            $("#email-auth-confirm").removeAttr("disabled");
                            $("#btn-email-auth-confirm").removeAttr("disabled");
                        } else {
                            $("#fg-mobile-req-btn-" + util.memType.toString()).hide();
                            $("#fg-mobile-cancel-btn-" + util.memType.toString()).show();
                            $("#fg-mobile-auth-btn-" + util.memType.toString()).show();
                            $("#mobile-" + util.memType.toString()).attr("disabled", "disabled");
                            $("#confirm-code-" + util.memType.toString()).removeAttr("disabled");
                        }
                    } else if (mode === "CONFIRM") {
                        $.commonUtils.redirectAlert(ka.msg.join.reqCodeOk, "success", "/Member/JoinStep2/" + util.authSequence);
                    }
                    return true;
                } else {
                    if (auth === "E") {
                        $("#email-auth-input-error").css("color", "red");
                        util.validateMsg("email-auth-input", eval(result.code));
                    } else {
                        util.validateMsg("mobile-cont-" + $.joinUtils.memType.toString(), eval(result.code));
                    }
                    return false;
                }
            },

            /*------------------------------------------------------------------
                * @function:    setMobileForeignAuth
                * @description: 해외개인고객/해외법인고객 휴대전화 인증 요청 및 확인 처리
                ------------------------------------------------------------------*/
            setMobileForeignAuth: function (mode, type, auth) {
                const util = $.joinUtils;
                //if (mode === "CONFIRM" && $("#confirm-code-" + util.memType.toString()).val() === "") {
                //    $.joinUtils.validateMsg("mobile-cont-" + util.memType.toString(), ka.msg.join.reqCodeEmpty);
                //    return false;
                //}
                if (mode === "CONFIRM") {
                    if (
                        auth === "M" &&
                        $("#confirm-code-" + util.memType.toString()).val() === ""
                    ) {
                        $.joinUtils.validateMsg(
                            "mobile-cont-" + util.memType.toString(),
                            ka.msg.join.reqCodeEmpty
                        );
                        return false;
                    }
                    if (auth === "E" && $("#email-auth-confirm").val() === "") {
                        $.joinUtils.validateMsg(
                            "email-auth-input",
                            ka.msg.join.reqCodeEmpty
                        );
                        return false;
                    }
                }

                var param = {};
                param["mode"] = mode;
                param["seq"] = util.authSequence;
                param["type"] = typeof type === "string" && type !== "" ? type : "J";
                param["type_detail"] =
                    util.memType.toString().length > 1
                        ? util.memType.toString()
                        : "00" + util.memType.toString();
                param["auth"] = auth;

                // 국가코드 처리 추가
                if (auth === "E") {
                    param["mobile_no"] = $("#email-auth-input").val();
                    param["auth_no"] = $("#email-auth-confirm").val();
                } else {
                    var countryPhoneCode = $(".overseas-individuals-member").find($(".iti__selected-dial-code")).html();

                    param["mobile_no"] =
                        countryPhoneCode.replace(/\+/gi, "") +
                        $("#mobile-" + util.memType.toString()).val();
                    param["auth_no"] = $(
                        "#confirm-code-" + util.memType.toString()
                    ).val();
                }

                var result = $.ajaxUtils.getApiData(
                    "/api/Member/SetMobileForeignAuth",
                    param,
                    null,
                    false
                );
                if (result.code === "00") {
                    util.authSequence = result.data.Sequence;
                    if (mode === "INSERT") {
                        if (
                            location.href.indexOf("www.dev.") > -1 ||
                            location.href.indexOf("www.release.") > -1 ||
                            location.href.indexOf("localhost") > -1
                        ) {
                            $.commonUtils.alert(
                                "테스트계는 인증번호를 제공 합니다.<br />인증번호 : " +
                                result.data.AuthNo,
                                "success"
                            );
                        }
                        if (auth === "E") {
                            $("#email-auth-input-error").css("color", "red");
                            $("#email-auth-input-error").html("");

                            $("#email-auth-input").attr("disabled", "disabled");
                            $("#btn-email-auth-request").attr("disabled", "disabled");
                            $("#email-auth-confirm").removeAttr("disabled");
                            $("#btn-email-auth-confirm").removeAttr("disabled");
                        } else {
                            $("#fg-mobile-req-btn-" + util.memType.toString()).hide();
                            $("#fg-mobile-cancel-btn-" + util.memType.toString()).show();
                            $("#fg-mobile-auth-btn-" + util.memType.toString()).show();
                            $("#mobile-" + util.memType.toString()).attr(
                                "disabled",
                                "disabled"
                            );
                            $("#confirm-code-" + util.memType.toString()).removeAttr(
                                "disabled"
                            );
                        }
                    } else if (mode === "CONFIRM") {
                        //$("#mobile-auth-" + util.memType.toString() + "-error").remove();
                        //$("#auth-remain-time-" + util.memType.toString()).html('');
                        //$("#confirm-code-" + util.memType.toString()).attr("disabled", "disabled");
                        //$("#fg-mobile-auth-num-" + util.memType.toString()).parent().addClass("disabled");
                        //$("#fg-mobile-auth-btn-" + util.memType.toString()).hide();
                        //$("#fg-mobile-cancel-btn-" + util.memType.toString()).hide();
                        //$("#auth-remain-time-" + util.memType.toString()).countdown('stop');
                        //$.joinUtils.authFlag[util.memType - 1] = true;
                        $.commonUtils.redirectAlert(
                            ka.msg.join.reqCodeOk,
                            "success",
                            "/Member/JoinStep2/" + util.authSequence
                        );
                        //$("#fg-mobile-retry-btn-" + util.memType.toString()).show();

                        //if (util.memType.toString() === "2") {
                        //    $("#fg-name-" + util.memType).removeClass("disabled");
                        //    $("#name-" + util.memType).removeAttr("disabled");
                        //    $("#fg-sex-" + util.memType).removeClass("disabled");
                        //    $("#sex-" + util.memType).removeAttr("disabled");
                        //    $("#fg-birth-date-" + util.memType).removeClass("disabled");
                        //    $("#birth-date-" + util.memType).removeAttr("disabled");
                        //}
                    }
                    return true;
                } else {
                    if (auth === "E") {
                        $("#email-auth-input-error").css("color", "red");
                        util.validateMsg("email-auth-input", eval(result.code));
                    } else {
                        util.validateMsg(
                            "mobile-cont-" + $.joinUtils.memType.toString(),
                            eval(result.code)
                        );
                    }
                    return false;
                }
            },

            /*------------------------------------------------------------------
              * @function:    setMobileForeignAuth
              * @description: 해외개인고객/해외법인고객 휴대전화 인증 요청 및 확인 처리
              ------------------------------------------------------------------*/
            retryMobileForeignAuth: function (memType) {
                $("#mobile-" + memType.toString()).removeAttr("disabled");
                $("#fg-mobile-" + memType.toString())
                    .parent()
                    .removeClass("disabled");
                $("#fg-mobile-req-btn-" + memType.toString()).show();
                $("#fg-mobile-cancel-btn-" + memType.toString()).hide();

                $("#confirm-code-" + memType.toString()).attr("disabled", "disabled");
                $("#fg-mobile-auth-" + memType.toString()).addClass("disabled");
                $("#fg-mobile-auth-btn-" + memType.toString()).hide();
                $("#fg-mobile-retry-btn-" + memType.toString()).hide();

                $.joinUtils.authFlag[memType - 1] = false;
            },

            /*------------------------------------------------------------------
              * @function:    openAuthResult
              * @param:       result (인증 처리 결과)
              *               message (인증 처리 결과 메세지)
              *               seq (인증 처리 seq)
              * @description: 인증 처리 결과에 따라 필드 활성화 처리 및 메세지 처리
              *               2022.05.09 :: 마이페이지 고도화에 따른 처리결과 UI 수정
              ------------------------------------------------------------------*/
            openAuthResult: function (result, message, seq, data) {
                // 모달닫기
                try {
                    document.querySelector(".mypage_modal.show").classList.remove("show");
                    document.querySelector("body").classList.remove("scroll_lock");
                } catch (e) { }

                if (result === "T") {
                    let param = {};
                    param["seq"] = seq;
                    const mobileAuth =
                        typeof data !== "undefined"
                            ? data
                            : $.ajaxUtils.getApiData(
                                "/api/Member/GetMobileAuth",
                                param,
                                null,
                                false
                            );

                    if (mobileAuth.data.length > 0) {
                        if ($.joinUtils.reqType === "A") {
                            // 신규약관 동의 시 휴대폰 인증
                            try {
                                requestProc(seq);
                            } catch (e) { }
                        } else {
                            //2022.05.02 :: 기존소스 주석
                            const checkFields =
                                $.joinUtils.reqType === "M"
                                    ? $.mypageUtils.fields[$.joinUtils.memType - 1]
                                    : $.joinUtils.fields[$.joinUtils.memType - 1];

                            //checkFields.forEach(function (item) {
                            //    $("#fg-" + item + "-" + $.joinUtils.memType).removeClass("disabled");
                            //    $("#" + item + "-" + $.joinUtils.memType).removeAttr("disabled");
                            //});

                            //const data = mobileAuth.data[0];
                            //$("#fg-name-" + $.joinUtils.memType).addClass("disabled");
                            //$("#name-" + $.joinUtils.memType).attr("disabled", true);
                            //$("#name-" + $.joinUtils.memType).val(data.mem_name);
                            //$("#name-" + $.joinUtils.memType).css("color", "#212121");

                            const data = mobileAuth.data[0];

                            $("#join-mobile-seq").val(seq);

                            $("#join-name").val(data.mem_name);
                            $("#auth-name").html(data.mem_name);

                            //if (data.birth_date.length === 8) {
                            //    data.birth_date = data.birth_date.replace(/(\d{4})(\d{2})(\d{2})/, '$1-$2-$3');
                            //}

                            //$("#fg-birth-date-" + $.joinUtils.memType).addClass("disabled");
                            //$("#birth-date-" + $.joinUtils.memType).attr("disabled", true);
                            //$("#birth-date-" + $.joinUtils.memType).val(data.birth_date);
                            //$("#birth-date-" + $.joinUtils.memType).css("color", "#212121");

                            $("#join-birth-date").val(
                                $.commonUtils.getBirthFormat(data.birth_date)
                            );
                            $("#auth-birth-date").html(
                                $.commonUtils
                                    .getBirthFormat(data.birth_date)
                                    .replace(/-/gi, ".")
                            );

                            //$("#fg-sex-" + $.joinUtils.memType).addClass("disabled");
                            //$("#sex-" + $.joinUtils.memType).attr("disabled", true);
                            //$("#sex-" + $.joinUtils.memType).val(data.gender.toString()).prop("selected", true);
                            //$("#select2-sex-" + $.joinUtils.memType + "-container").attr("title", data.gender.toString() === "M" ? "남자" : "여자");
                            //$("#select2-sex-" + $.joinUtils.memType + "-container").html(data.gender.toString() === "M" ? "남자" : "여자")

                            if ($.joinUtils.authType === "M") {
                                $("#fg-mobile-" + $.joinUtils.memType).addClass("disabled");
                                $("#mobile-" + $.joinUtils.memType).attr("disabled", true);
                                $("#mobile-" + $.joinUtils.memType).val(data.mobile_no);
                                $("#mobile-" + $.joinUtils.memType).css("color", "#212121");
                            }

                            $("#mobile").html(data.mobile_no);

                            //$("#fg-zipcode-" + $.joinUtils.memType).addClass("disabled");
                            //$("#zipcode-" + $.joinUtils.memType).attr("disabled", true);
                            //$("#fg-address-" + $.joinUtils.memType).addClass("disabled");
                            //$("#address-" + $.joinUtils.memType).attr("disabled", true);

                            //if ($.joinUtils.reqType === "M") {
                            //    $("#mobile-auth-" + $.joinUtils.memType + "-m").hide();
                            //    $("#mobile-auth-" + $.joinUtils.memType + "-c").hide();
                            //    $.mypageUtils.authFlag = true;

                            //    $("#authenticate-container").hide();
                            //} else {
                            //    $("#mobile-auth-" + $.joinUtils.memType).hide();
                            //}

                            //$("#join-mobile-auth-seq").val(seq);
                            //$("#join-verification").val($.joinUtils.authType);

                            //$.joinUtils.authFlag[$.joinUtils.memType - 1] = true;

                            $.joinUtils.authFlag[$.joinUtils.memType - 1] = true;
                        }
                    }
                } else {
                    $.commonUtils.alert(message);
                }
            },

            /*------------------------------------------------------------------
              * @function:    openAuthResultV2
              * @param:       result (인증 처리 결과)
              *               message (인증 처리 결과 메세지)
              *               seq (인증 처리 seq)
              * @description: 인증 처리 결과에 따라 필드 활성화 처리 및 메세지 처리
              ------------------------------------------------------------------*/
            openAuthResultV2: function (result, message, seq, data) {
                if (result === "T") {
                    //2022.07.25 :: 인증 처리 후 현재화면 새로 고침
                    //$.commonUtils.redirectAlert(
                    //    "인증되었습니다. 회원정보를 입력해주세요.",
                    //    "success",
                    //    "/Member/JoinStep2/" + seq
                    //);
                    $.commonUtils.alertWithFn("본인인증 및 회원정보가 업데이트 됐습니다.", "success", "window.location.reload();");
                } else {
                    $.commonUtils.alert(decodeURI(message));
                }
            },

            /*------------------------------------------------------------------
              * @function:    openAuthResultV3
              * @param:       result (인증 처리 결과)
              *               message (인증 처리 결과 메세지)
              *               url (인증 후 이동할 경로)
              *               msg (표시 정보)
              * @description: 인증 후 정보가 변경된 경우 메세지 처리
              ------------------------------------------------------------------*/
            openAuthResultV3: function (result, message, url, msg) {
                if (result === "T") {
                    $.commonUtils.redirectAlert((msg && msg !== '') ? '회원명이 인증 된 이름으로 업데이트 됩니다.​<br />' + msg : "인증되었습니다.", "success", url);
                } else {
                    $.commonUtils.redirectAlert("인증시 오류가 발생하였습니다.", "error", url);
                }
            },

            setAuthResult: function (seq, reqType) {
                let param = {};
                param["seq"] = seq;
                const mobileAuth = $.ajaxUtils.getApiData(
                    "/api/Member/GetMobileAuth",
                    param,
                    null,
                    false
                );
                if (mobileAuth.data.length > 0) {
                    const mobileAuthData = mobileAuth.data[0];
                    $.joinUtils.memType = parseInt(mobileAuthData.type_detail, 10);
                    $.joinUtils.authType = mobileAuthData.auth;

                    if (reqType && reqType === "M") {
                        $.joinUtils.reqType = reqType;
                        $("#mobile-auth-" + $.joinUtils.memType).hide();
                    }

                    $.joinUtils.openAuthResult("T", "", seq, mobileAuth);
                    $("#check-authentication-" + $.joinUtils.memType.toString()).trigger(
                        "click"
                    );
                }
            },

            /*------------------------------------------------------------------
              * @function:    fieldsEnable
              * @param:       memType (회원유형)
              * @description: 유형에 맞는 필드들 입력 활성화 처리
              ------------------------------------------------------------------*/
            fieldsEnable: function (memType) {
                $.joinUtils.fields[memType - 1].forEach(function (item) {
                    $("#fg-" + item + "-" + memType).removeClass("disabled");
                    $("#" + item + "-" + memType).removeAttr("disabled");
                });
            },

            /*------------------------------------------------------------------
              * @function:    validate
              * @description: 가입 하기 클릭 시 입력값 체크
              ------------------------------------------------------------------*/
            validate: function () {
                let errorFlag = false;
                const utils = $.joinUtils;
                const joinTypeIndex = parseInt(utils.joinType, 10);

                try {
                    utils.joinType = $(
                        "input:radio[name=check-authentication]:checked"
                    ).val();
                    if (typeof utils.joinType === "undefined" || utils.joinType === "") {
                        $.commonUtils.alert(ka.msg.join.memTypeEmpty);
                        return false;
                    }

                    if (utils.joinType === "001" || utils.joinType === "003") {
                        // 1. 국내개인고객, 3. 국내법인고객
                        if (
                            (utils.joinType === "001" && !utils.authFlag[0]) ||
                            (utils.joinType === "003" && !utils.authFlag[2])
                        ) {
                            $.commonUtils.alert(ka.msg.join.memAuth);
                            return false;
                        }
                    } else if (utils.joinType === "002" || utils.joinType === "004") {
                        if (
                            (utils.joinType === "002" && !utils.authFlag[1]) ||
                            (utils.joinType === "004" && !utils.authFlag[3])
                        ) {
                            $.commonUtils.alert(ka.msg.join.reqCodeRequire);
                            return false;
                        }
                    }

                    // 만19세 이하 체크
                    if (
                        utils.checkAge($("#birth-date-" + joinTypeIndex.toString()).val()) <
                        19
                    ) {
                        $.commonUtils.alert(ka.msg.join.memAgeRule);
                        return false;
                    }

                    // 약관 동의 확인
                    var agreements = $("#agreements").find("input");
                    var privateClause = "N";
                    var clauseInfo = "";
                    var receivingAdvertising = "N";

                    for (var i = 0; i < agreements.length; i++) {
                        if (
                            agreements[i].dataset.require === "Y" &&
                            !agreements[i].checked
                        ) {
                            $.commonUtils.alert(ka.msg.join.memAgreementEmpty);
                            return false;
                        }

                        privateClause =
                            agreements[i].dataset.smsMailYn === "P" && agreements[i].checked
                                ? "Y"
                                : "N";
                        receivingAdvertising =
                            agreements[i].dataset.smsMailYn === "Y" && agreements[i].checked
                                ? "Y"
                                : "N";

                        clauseInfo +=
                            (clauseInfo !== "" ? "|" : "") +
                            agreements[i].dataset.code +
                            "^" +
                            (agreements[i].checked ? "Y" : "N");
                    }
                    $("#join-clause-info").val(clauseInfo);
                    $("#join-receiving-advertising").val(receivingAdvertising);

                    var target = parseInt($.joinUtils.joinType, 10) - 1;
                    $.joinUtils.joinMsg = "";
                    $.joinUtils.fields[target].forEach(function (item) {
                        try {
                            if ($.joinUtils.joinMsg !== "") return false;

                            if (
                                $("#fg-" + item + "-" + (target + 1))
                                    .get(0)
                                    .className.indexOf("required") > 0 &&
                                $("#" + item + "-" + (target + 1)).val() === ""
                            ) {
                                $.joinUtils.joinMsg = $.commonUtils.alert(
                                    $("#" + item + "-" + (target + 1)).get(0).dataset.msg
                                );
                            } else {
                                $("#join-" + item).val(
                                    $("#" + item + "-" + (target + 1)).val()
                                );
                            }
                        } catch (e) {
                            console.log(e.description);
                            errorFlag = true;
                        }
                    });

                    // 첨부파일 확장자 체크
                    if (utils.joinType === "002") {
                        if (
                            !utils.validateUploadFile(
                                $("#identification-file-" + joinTypeIndex).val()
                            )
                        ) {
                            $.commonUtils.alert(ka.msg.join.fileExtension);
                            return false;
                        }
                    } else if (utils.joinType === "003" || utils.joinType === "004") {
                        if (
                            !utils.validateUploadFile(
                                $("#company-reg-doc-file-" + joinTypeIndex).val()
                            ) ||
                            !utils.validateUploadFile(
                                $("#company-business-card-file-" + joinTypeIndex).val()
                            )
                        ) {
                            $.commonUtils.alert(ka.msg.join.fileExtension);
                            return false;
                        }
                    }

                    // 이메일/비밀번호 유효성 체크
                    if (
                        !$.joinUtils.checkEmail(
                            "email-" + +parseInt($.joinUtils.joinType, 10)
                        )
                    )
                        return false;
                    if (
                        !$.joinUtils.checkPassword(
                            "pwd-" + +parseInt($.joinUtils.joinType, 10)
                        )
                    )
                        return false;
                    if (
                        !$.joinUtils.checkPassword(
                            "pwd2-" + +parseInt($.joinUtils.joinType, 10)
                        )
                    )
                        return false;

                    // 휴대폰 중복 체크
                    if (
                        $.joinUtils.joinType === "001" ||
                        $.joinUtils.joinType === "002"
                    ) {
                        if (
                            !$.joinUtils.serverCheckMobile($("#mobile-" + (target + 1)).val())
                        ) {
                            $.joinUtils.joinMsg = ka.msg.join.memExistMobile;
                        }
                    }

                    if (!errorFlag && $.joinUtils.joinMsg === "") {
                        $.commonUtils.confirm(
                            ka.msg.join.title,
                            ka.msg.join.complete,
                            "$.joinUtils.validateConfirm('" +
                            $.joinUtils.joinType +
                            "', '" +
                            privateClause +
                            "');"
                        );
                        return false;
                    } else {
                        if ($.joinUtils.joinMsg !== "")
                            $.commonUtils.alert($.joinUtils.joinMsg);
                        return false;
                    }
                } catch (e) {
                    console.log(e.message);
                    return false;
                }
            },

            setValiateFlag: function (flag, msg) {
                if (flag && $.joinUtils.joinMsg === "") {
                    $.joinUtils.validateFlag = flag;
                    $.joinUtils.joinMsg = msg;
                } else if (!flag) {
                    $.joinUtils.validateFlag = flag;
                    $.joinUtils.joinMsg = "";
                }
            },

            /*------------------------------------------------------------------
              * @function:    validateV2
              * @description: 가입 하기 클릭 시 입력값 체크 (신규)
              ------------------------------------------------------------------*/
            validateV2: function () {
                let utils = $.joinUtils;
                utils.validateFlag = false;
                utils.joinMsg = "";
                utils.joinType = $("#join-type").val();

                // 회원유형 체크
                if (typeof utils.joinType === "undefined" || utils.joinType === "") {
                    utils.setValiateFlag(true, ka.msg.join.memTypeEmptyNotice);
                }

                // 필드 값 체크
                utils.field.forEach(function (item) {
                    try {
                        if (utils.joinMsg !== "") return false;
                        if (
                            $("#" + item)
                                .val()
                                .replace(/ /gi, "") === ""
                        ) {
                            utils.setValiateFlag(true, $("#" + item).get(0).dataset.msg);
                        } else {
                            $("#join-" + item).val($("#" + item).val());
                        }
                    } catch (e) {
                        utils.setValiateFlag(true, ka.msg.common.error);
                    }
                });

                // 해외 개인
                if (utils.joinType === "002") {
                    if ($("#identification").val() !== "") {
                        $("#join-identification").val($("#identification").val());
                    } else {
                        utils.setValiateFlag(true, $("#identification").get(0).dataset.msg);
                    }
                }

                // 법인 필드 값 체크
                if (utils.joinType === "003" || utils.joinType === "004") {
                    // 사업자 등록번호 인증확인 여부
                    if (utils.joinType === "003" && !utils.validateBusinessNum) {
                        utils.setValiateFlag(true, "사업자등록번호를 인증해주세요.");
                    }

                    // 필드 값 체크
                    utils.corpField.forEach(function (item) {
                        try {
                            if (utils.joinMsg !== "") return false;
                            if (
                                $("#" + item)
                                    .val()
                                    .replace(/ /gi, "") === ""
                            ) {
                                utils.setValiateFlag(true, $("#" + item).get(0).dataset.msg);
                            } else {
                                $("#join-" + item).val($("#" + item).val());
                            }
                        } catch (e) {
                            utils.setValiateFlag(true, ka.msg.common.error);
                        }
                    });

                    // 첨부파일 확장자 체크
                    if (
                        !utils.validateUploadFile($("#join-company-reg-doc-file").val()) ||
                        !utils.validateUploadFile(
                            $("#join-company-business-card-file").val()
                        )
                    ) {
                        utils.setValiateFlag(true, ka.msg.join.fileExtension);
                    }
                }

                // 비밀번호 입력값 체크
                if (
                    !$.joinUtils.checkPassword("pwd") ||
                    !$.joinUtils.checkPassword("pwd-re")
                ) {
                    utils.setValiateFlag(true, ka.msg.join.memPwdRuleLength);
                }

                // 비밀번호/비밀번호(확인) 체크
                if ($("#pwd").val() !== $("#pwd-re").val()) {
                    utils.setValiateFlag(true, ka.msg.join.memPwdRuleSame);
                }

                // 이메일 유효성 체크
                if (!$.validateUtils.checkEmail("email")) {
                    utils.setValiateFlag(true, ka.msg.join.memEmailRule);
                }

                // 휴대폰 중복 체크
                if (utils.joinType === "001" || utils.joinType === "002") {
                    if (!utils.serverCheckMobile($("#mobile").val())) {
                        utils.joinMsg = ka.msg.join.memExistMobile;
                    }
                }

                // 만19세 이하 체크
                if (utils.checkAge($("#birth-date").val()) < 19) {
                    utils.setValiateFlag(true, ka.msg.join.memAgeRule);
                }

                // 개인정보 수집/이용 (주소)
                if (utils.joinType === "001" || utils.joinType === "002") {
                    var el = $("#join-collect-info-address").find("div[class='on']");
                    if (el.length > 0 && el.get(0).dataset.value !== "") {
                        $("#join-collect-personal-info-address-yn").val(
                            el.get(0).dataset.value
                        );
                    } else {
                        utils.setValiateFlag(
                            true,
                            ka.msg.join.collectPersonalInfoAddressNotSelect
                        );
                    }

                    // 개인정보 수집/이용 - 주소 동의
                    if ($("#join-collect-personal-info-address-yn").val() === "Y") {
                        // 주소
                        let extraField = ["country-code", "zipcode", "address", "address2"];
                        extraField.forEach(function (item) {
                            if (utils.joinMsg !== "") return false;

                            //if ($("#" + item).val() !== "") {
                            //    $("#join-" + item).val($("#" + item).val());
                            //}
                            if (
                                $("#" + item)
                                    .val()
                                    .replace(/ /gi, "") === ""
                            ) {
                                utils.setValiateFlag(true, $("#" + item).get(0).dataset.msg);
                            } else {
                                $("#join-" + item).val($("#" + item).val());
                            }
                        });

                        // 제3자 제공 동의
                        if ($("#zipcode-conditions-all-agree").is(":checked")) {
                            $("#join-provide-personal-info-agree-yn").val("Y");
                        }
                    }
                } else {
                    // 주소
                    let extraField = ["country-code", "zipcode", "address", "address2"];
                    extraField.forEach(function (item) {
                        if (utils.joinMsg !== "") return false;

                        if ($("#" + item).length > 0) {
                            if (
                                $("#" + item)
                                    .val()
                                    .replace(/ /gi, "") === ""
                            ) {
                                utils.setValiateFlag(true, $("#" + item).get(0).dataset.msg);
                            } else {
                                $("#join-" + item).val($("#" + item).val());
                            }
                        } else {
                            if (item === "country-code") {
                                $("#join-" + item).val("KOR");
                            }
                        }
                    });
                }

                // 개인정보 수집/이동 (맞춤 컨텐츠)
                var el = $("#join-agree-info-content").find("div[class='on']");
                if (el.length > 0 && el.get(0).dataset.value !== "") {
                    $("#join-collect-personal-info-content-yn").val(
                        el.get(0).dataset.value
                    );
                } else {
                    utils.setValiateFlag(
                        true,
                        ka.msg.join.collectPersonalInfoContentNotSelect
                    );
                }

                // 개인정보 수집/이용 - 컨텐츠 동의
                if ($("#join-collect-personal-info-content-yn").val() === "Y") {
                    // 관심분야
                    var interest = "";
                    $.each(
                        $("input[name='attention_part_contents_checkbox']"),
                        function (index, item) {
                            if (item.checked) {
                                interest += interest === "" ? item.value : "," + item.value;
                            }
                        }
                    );
                    $("#join-interest-area").val(interest);

                    // 관심작가
                    let interestArtist = "";
                    $.each($(".artist_selected"), function (index, item) {
                        interestArtist +=
                            interestArtist === "" ? item.dataset.uid : "," + item.dataset.uid;
                    });
                    $("#join-interest-artist").val(interestArtist);

                    // 성별
                    var genderCheck = $("input[name='gender_checkbox']").is(":checked");
                    if (genderCheck) {
                        $("#join-sex").val(
                            $("input[name='gender_checkbox']:checked").val()
                        );
                    }

                    // 직업
                    $("#join-job-code").val($("#join_job_select").val());
                    $("#join-company-name").val($("#company_name").val());
                    $("#join-company-tel").val($("#company_tel").val());
                }

                // 광고성 정보 수신
                $("#join-receive-email-info").val(
                    $("#join_ad_info_01").is(":checked") ? "Y" : "N"
                );
                $("#join-receive-sms-info").val(
                    $("#join_ad_info_02").is(":checked") ? "Y" : "N"
                );
                $("#join-receive-phone-info").val(
                    $("#join_ad_info_03").is(":checked") ? "Y" : "N"
                );

                if (!utils.validateFlag && utils.joinMsg === "") {
                    $.commonUtils.confirm(
                        ka.msg.join.title,
                        ka.msg.join.complete,
                        "$.joinUtils.validateV2Proc();"
                    );
                } else {
                    if (utils.joinMsg !== "") $.commonUtils.alert($.joinUtils.joinMsg);
                }
            },

            validateV2Proc: function () {
                $.commonUtils.loadingAni(document.getElementsByTagName("body")[0]);
                $("#form-registration").submit();
            },

            checkBusinessNum: function (target, mode) {
                var businessNum = $("#" + target).val();
                if (businessNum.replace(/ /gi, "") === "") {
                    $.joinUtils.validateMsg(target, "사업자등록번호를 입력하세요.");
                    $.joinUtils.checkClass(
                        document.getElementById("company-reg-no-error"),
                        "nagative",
                        "nagative"
                    );
                    return false;
                }
                var result = $.validateUtils.checkBusinessNum(businessNum);
                if (result) {
                    if (mode === "J") {
                        $("#" + target).attr("readonly", "readonly");
                        $("#" + target).attr("onfocus", "return false;");
                        $("#" + target + "-btn").css("opacity", "0.5");
                        $("#" + target + "-btn").attr("onclick", "return false;");
                        $.joinUtils.validateBusinessNum = true;
                        $.joinUtils.validateMsg(
                            target,
                            "국세청 데이터로 등록된 번호입니다."
                        );
                        $.joinUtils.checkClass(
                            document.getElementById("company-reg-no-error"),
                            "nagative",
                            "positive"
                        );
                    }
                } else {
                    $.joinUtils.validateBusinessNum = false;
                    $.joinUtils.validateMsg(
                        target,
                        "국세청 데이터에 존재하지 않는 번호입니다."
                    );
                    $.joinUtils.checkClass(
                        document.getElementById("company-reg-no-error"),
                        "nagative",
                        "nagative"
                    );
                }
            },

            /*------------------------------------------------------------------
              * @function:    validateUploadFile
              * @description: 업로드 가능 여부 확장자 체크 (jpg, png, pdf)
              ------------------------------------------------------------------*/
            validateUploadFile: function (fileName) {
                var ext = fileName.substring(fileName.lastIndexOf("."));
                if (
                    ext === "" ||
                    (ext.toLowerCase() !== ".jpg" &&
                        ext.toLowerCase() !== ".jpeg" &&
                        ext.toLowerCase() !== ".heic" &&
                        ext.toLowerCase() !== ".png" &&
                        ext.toLowerCase() !== ".pdf")
                ) {
                    return false;
                } else {
                    return true;
                }
            },

            validateConfirm: function (joinType, privateCaluse) {
                $("#join-type").val(joinType);
                $("#join-private-clause").val(privateCaluse);
                $("#form-registration").submit();
            },

            showZipCodeLayer: function (
                targetId,
                targetZipCode,
                targetAddress,
                targetAddressDetail
            ) {
                $.joinUtils.element_wrap = document.getElementById(targetId);

                // 현재 scroll 위치를 저장해놓는다.
                var currentScroll = Math.max(
                    document.body.scrollTop,
                    document.documentElement.scrollTop
                );
                new daum.Postcode({
                    oncomplete: function (data) {
                        // 검색결과 항목을 클릭했을때 실행할 코드를 작성하는 부분.

                        // 각 주소의 노출 규칙에 따라 주소를 조합한다.
                        // 내려오는 변수가 값이 없는 경우엔 공백('')값을 가지므로, 이를 참고하여 분기 한다.
                        var addr = ""; // 주소 변수
                        var extraAddr = ""; // 참고항목 변수

                        //사용자가 선택한 주소 타입에 따라 해당 주소 값을 가져온다.
                        if (data.userSelectedType === "R") {
                            // 사용자가 도로명 주소를 선택했을 경우
                            addr = data.roadAddress;
                        } else {
                            // 사용자가 지번 주소를 선택했을 경우(J)
                            addr = data.jibunAddress;
                        }

                        // 사용자가 선택한 주소가 도로명 타입일때 참고항목을 조합한다.
                        if (data.userSelectedType === "R") {
                            // 법정동명이 있을 경우 추가한다. (법정리는 제외)
                            // 법정동의 경우 마지막 문자가 "동/로/가"로 끝난다.
                            if (data.bname !== "" && /[동|로|가]$/g.test(data.bname)) {
                                extraAddr += data.bname;
                            }
                            // 건물명이 있고, 공동주택일 경우 추가한다.
                            if (data.buildingName !== "" && data.apartment === "Y") {
                                extraAddr +=
                                    extraAddr !== ""
                                        ? ", " + data.buildingName
                                        : data.buildingName;
                            }
                            // 표시할 참고항목이 있을 경우, 괄호까지 추가한 최종 문자열을 만든다.
                            if (extraAddr !== "") {
                                extraAddr = " (" + extraAddr + ")";
                            }

                            //// 조합된 참고항목을 해당 필드에 넣는다.
                            //document.getElementById("sample3_extraAddress").value = extraAddr;
                        } else {
                            // document.getElementById("sample3_extraAddress").value = '';
                        }

                        // 우편번호와 주소 정보를 해당 필드에 넣는다.
                        document.getElementById(targetZipCode).value = data.zonecode;
                        document.getElementById(targetAddress).value = addr;
                        // 커서를 상세주소 필드로 이동한다.
                        document.getElementById(targetAddressDetail).focus();

                        // iframe을 넣은 element를 안보이게 한다.
                        // (autoClose:false 기능을 이용한다면, 아래 코드를 제거해야 화면에서 사라지지 않는다.)
                        $.joinUtils.element_wrap.style.display = "none";

                        // 우편번호 찾기 화면이 보이기 이전으로 scroll 위치를 되돌린다.
                        document.body.scrollTop = currentScroll;
                    },
                    // 우편번호 찾기 화면 크기가 조정되었을때 실행할 코드를 작성하는 부분. iframe을 넣은 element의 높이값을 조정한다.
                    onresize: function (size) {
                        $.joinUtils.element_wrap.style.height = size.height - 150 + "px";
                    },
                    width: "100%",
                    height: "100%",
                }).embed($.joinUtils.element_wrap);

                // iframe을 넣은 element를 보이게 한다.
                $.joinUtils.element_wrap.style.display = "block";
            },

            hideZipCodeLayer: function () {
                if ($.joinUtils.element_wrap !== null)
                    $.joinUtils.element_wrap.style.display = "none";
            },

            /*------------------------------------------------------------------
              * @function:    serverCheckMobile
              * @param:       mobile (입력받은 모바일)
              * @description: 모바일 입력 값 중복 유무 서버 체크
              ------------------------------------------------------------------*/
            serverCheckMobile: function (mobile) {
                var param = {};
                param["mobile"] = mobile;
                return $.ajaxUtils.getApiData(
                    "/api/Member/CheckMobile",
                    param,
                    null,
                    false
                );
            },

            checkClass: function (target, remove, add) {
                if (target !== null) {
                    target.classList.remove(remove);
                    target.classList.add(add);
                }
            },
            /*------------------------------------------------------------------
              * @function:    checkID
              * @description: 아이디 입력 값 존재유무 체크
              ------------------------------------------------------------------*/
            checkID: function (obj) {
                var targetID = obj.id;

                if (
                    !$.validateUtils.regExp($("#" + targetID), $.validateUtils.opt.memID)
                ) {
                    $.joinUtils.validateMsg(targetID, ka.msg.join.memIdRule);
                    $.joinUtils.checkClass(
                        document.getElementById(targetID + "-error"),
                        "positive",
                        "nagative"
                    );
                } else {
                    if ($.joinUtils.serverCheckID(obj.value)) {
                        $.joinUtils.validateMsg(targetID, ka.msg.join.memIdNotUsed);
                        $.joinUtils.checkClass(
                            document.getElementById(targetID + "-error"),
                            "nagative",
                            "positive"
                        );
                    } else {
                        $.joinUtils.validateMsg(targetID, ka.msg.join.memIdUsed);
                        $.joinUtils.checkClass(
                            document.getElementById(targetID + "-error"),
                            "nagative",
                            "nagative"
                        );
                    }
                }
            },

            /*------------------------------------------------------------------
              * @function:    checkPassword
              * @description: 비밀번호 입력 값 유효성 체크
              ------------------------------------------------------------------*/
            checkPassword: function (target) {
                var value = $("#" + target).val();
                var num = value.search(/[0-9]/g);
                var eng = value.search(/[a-z]/gi);
                var spe = value.search(/[`~!@@#$%^&*|₩₩₩'₩";:₩/?]/gi);

                $.joinUtils.validateMsg(target, "");
                $.joinUtils.checkClass(
                    document.getElementById(target + "-error"),
                    "nagative",
                    "nagative"
                );

                if (value.length < 10 || value.length > 20) {
                    $.joinUtils.validateMsg(target, ka.msg.join.memPwdRuleLength);
                    return false;
                } else if (value.search(/\s/) >= 0) {
                    $.joinUtils.validateMsg(target, ka.msg.join.memPwdRuleEmpty);
                    return false;
                } else if (num < 0 || (eng < 0 && spe < 0)) {
                    $.joinUtils.validateMsg(target, ka.msg.join.memPwdRuleCheck);
                    return false;
                } else if (
                    target.indexOf("pwd2-") > -1 ||
                    target.indexOf("Confirm") > -1 ||
                    target.indexOf("-re") > -1
                ) {
                    var valueOrg = $(
                        "#" +
                        target
                            .replace("pwd2-", "pwd-")
                            .replace("Confirm", "")
                            .replace("-re", "")
                    ).val();
                    if (value !== valueOrg) {
                        $.joinUtils.validateMsg(target, "내용이 일치하지 않습니다.");
                        return false;
                    }
                }

                $.joinUtils.validateMsg(
                    target,
                    target.indexOf("pwd2-") > -1 ||
                        target.indexOf("Confirm") > -1 ||
                        target.indexOf("-re") > -1
                        ? ka.msg.join.memPwdRuleSameVerified
                        : ka.msg.join.memPwdRuleType
                );
                $.joinUtils.checkClass(
                    document.getElementById(target + "-error"),
                    "nagative",
                    "positive"
                );
                return true;
            },

            /*------------------------------------------------------------------
              * @function:    checkEmail
              * @description: 이메일 입력 값 유효성 체크
              ------------------------------------------------------------------*/
            checkEmail: function (target) {
                var value = $("#" + target).val();
                var regExp =
                    /^[0-9a-zA-Z]([-_\.]?[0-9a-zA-Z_])*@[0-9a-zA-Z]([-_\.]?[0-9a-zA-Z])*\.[a-zA-Z]{2,3}$/i;

                var result = regExp.test(value);
                if (result) {
                    if (target === "email-auth-input") {
                        $("#email-auth-input-error").css("color", "blue");
                    }
                    $.joinUtils.checkClass(
                        document.getElementById(target + "-error"),
                        "nagative",
                        "positive"
                    );
                    $.joinUtils.validateMsg(target, ka.msg.join.memEmailRuleFormat);
                    return true;
                } else {
                    if (target === "email-auth-input") {
                        $("#email-auth-input-error").css("color", "red");
                    }
                    $.joinUtils.checkClass(
                        document.getElementById(target + "-error"),
                        "nagative",
                        "nagative"
                    );
                    $.joinUtils.validateMsg(target, ka.msg.join.memEmailRule);
                    return false;
                }
            },

            checkEmpty: function (target) {
                let el = $("#" + target);
                if (el.length > 0) {
                    let value = el.val().replace(/ /gi, "");
                    if (value === "" && el.get(0).dataset.msg !== null) {
                        $.joinUtils.validateMsg(target, el.get(0).dataset.msg);
                    } else {
                        $.joinUtils.validateMsg(target, "");
                    }
                }
            },

            /*------------------------------------------------------------------
              * @function:    checkCountry
              * @description: 주소 버튼 활성/비활성화 처리
              ------------------------------------------------------------------*/
            checkCountry: function (type, obj) {
                if (type === 1) {
                    if (obj.value === "KOR") {
                        $("#address-btn-" + type.toString()).show();
                        $("#fg-zipcode-" + type.toString()).addClass("disabled");
                        $("#zipcode-" + type.toString()).attr("disabled", true);
                        $("#fg-address-" + type.toString()).addClass("disabled");
                        $("#address-" + type.toString()).attr("disabled", true);
                    } else {
                        $("#address-btn-" + type.toString()).hide();
                        $("#fg-zipcode-" + type.toString()).removeClass("disabled");
                        $("#zipcode-" + type.toString()).attr("disabled", false);
                        $("#fg-address-" + type.toString()).removeClass("disabled");
                        $("#address-" + type.toString()).attr("disabled", false);
                        $.joinUtils.hideZipCodeLayer();
                    }
                    $("#zipcode-" + type.toString()).val("");
                    $("#address-" + type.toString()).val("");
                }
            },

            /*------------------------------------------------------------------
              * @function:    checkAge
              * @param:       value (YYYY-MM-DD 포맷)
              * @description: 만나이 계산
              ------------------------------------------------------------------*/
            checkAge: function (value) {
                const today = new Date();
                const inputBirthDate = new Date(value);

                let age = today.getFullYear() - inputBirthDate.getFullYear();
                const month = today.getMonth() - inputBirthDate.getMonth();
                if (
                    month < 0 ||
                    (month === 0 && today.getDate() < inputBirthDate.getDate())
                ) {
                    age--;
                }
                return age;
            },

            /*------------------------------------------------------------------
              * @function:    checkID
              * @param:       id (입력받은 아이디)
              * @description: 아이디 입력 값 존재유무 서버 체크
              ------------------------------------------------------------------*/
            serverCheckID: function (id) {
                var param = {};
                param["id"] = id;
                return $.ajaxUtils.getApiData(
                    "/api/Member/CheckID",
                    param,
                    null,
                    false
                );
            },

            /*------------------------------------------------------------------
              * @function:    validateMsg
              * @param:       target (대상 input id)
              *               msg (표기할 메세지)
              * @description: 대상 INPUT 하단에 메세지 표기 처리
              ------------------------------------------------------------------*/
            validateMsg: function (target, msg) {
                var error = $("#" + target + "-error");
                if (error.length > 0) {
                    if (msg !== "") {
                        error.html(msg);
                        error.css("display", "block");
                        error.show();
                    } else {
                        // error.remove();
                        error.html("");
                        error.hide();
                    }
                } else {
                    if (msg !== "") {
                        $("#fg-" + target)
                            .parent()
                            .append(
                                $("<div />", {
                                    id: target + "-error",
                                    class: "error",
                                    style:
                                        "line-height: 20px; font-size:12px; color: #b91e1e; margin-top: -10px; padding:3px 7px 2px 4px",
                                }).append(msg)
                            );
                    }
                }
            },

            handleFile: function (obj, target) {
                if (obj.length > 0) {
                    var pattern = /[\{\}\/?,;:|*~`!^\+<>@\#$%&\\\=\'\"]/gi;
                    if (pattern.test(obj[0].name)) {
                        $.commonUtils.alert(ka.msg.fileNameCheck);
                    } else {
                        if (
                            $.commonUtils.getFileExt(obj[0].name).toLowerCase() !== "heic" &&
                            $.commonUtils.getFileExt(obj[0].name).toLowerCase() !== "jpg" &&
                            $.commonUtils.getFileExt(obj[0].name).toLowerCase() !== "jpeg" &&
                            $.commonUtils.getFileExt(obj[0].name).toLowerCase() !== "png" &&
                            $.commonUtils.getFileExt(obj[0].name).toLowerCase() !== "pdf"
                        ) {
                            $.commonUtils.alert(ka.msg.join.fileExtension);
                        } else {
                            $("#" + target).val(obj[0].name);
                        }
                    }
                }
            },

            /*------------------------------------------------------------------
              * @function:    extraSave
              * @description: 가입 후 추가 정보 저장 처리 (JoinComplete)
              ------------------------------------------------------------------*/
            extraSave: function (target) {
                $.joinUtils.extraParam = {};
                $.joinUtils.extraFields[target - 1].forEach(function (item) {
                    $.joinUtils.extraParam[item.replace(/-/gi, "_")] = $(
                        "#" + item + "-" + target.toString()
                    ).val();
                });
                $.commonUtils.confirm(
                    ka.msg.common.save,
                    ka.msg.join.memExtraSave,
                    "$.joinUtils.extraSaveConfirm(" + target.toString() + ");"
                );
            },

            /*------------------------------------------------------------------
              * @function:    extraSaveConfirm
              * @param:       target (추가정보 영역 구분값 1:국내개인)
              * @description: 추가 정보 저장 결과 처리 (JoinComplete)
              ------------------------------------------------------------------*/
            extraSaveConfirm: function (target) {
                var result = $.ajaxUtils.getApiData(
                    "/api/Member/SetExtraInfo",
                    $.joinUtils.extraParam,
                    null,
                    false
                );
                try {
                    if (result.code === "00") {
                        $.commonUtils.alert(ka.msg.common.saveComplete, "success");
                        // $("#etc-info-" + target.toString()).hide();
                        window.location.href = "/";
                    } else {
                        $.commonUtils.alert(result.message);
                    }
                } catch (e) {
                    console.log(e.description);
                }
            },
        }),
        ($.mypageUtils = {
            process: false,
            selectedUid: null,
            procType: "",
            modifyMsg: null,
            authFlag: false,
            authSeq: "",
            fields: [
                // ["name", "birth-date", "sex", "mobile", "email", "country-code", "job-code", "company-name", "company-tel", "company-fax"],
                [
                    "name",
                    "birth-date",
                    "mobile",
                    "email",
                    "country-code",
                    "job-code",
                    "company-name",
                    "company-tel",
                    "company-fax",
                ],
                [
                    "email",
                    "country-code",
                    "job-code",
                    "company-name",
                    "company-tel",
                    "company-fax",
                ],
                [
                    "name",
                    "birth-date",
                    "sex",
                    "mobile",
                    "email",
                    "company-name",
                    "company-president",
                    "company-tel",
                    "company-rep-tel",
                    "company-reg-no",
                ],
                [
                    "name",
                    "birth-date",
                    "sex",
                    "mobile",
                    "email",
                    "company-name",
                    "company-president",
                    "country-code",
                    "company-tel",
                    "company-reg-no",
                ],
            ],
            addressFields: [
                "modal-country-code",
                "modal-zip-code",
                "modal-address1",
                "modal-address2",
                "modal-receiver",
                "modal-contact",
            ],
            requestParam: null,
            mobileValidationState: "",

            /*------------------------------------------------------------------
              * @function:    validatePassword
              * @description: 정보수정 - 비밀번호 수정 클릭 시 입력 값 유효성 체크
              ------------------------------------------------------------------*/
            validatePassword: function () {
                var validate = $.validateUtils;
                if ($("#password-old").val().replace(/ /g, "") === "") {
                    $.commonUtils.alertWithFn(
                        ka.msg.join.memPwdRuleEmpty,
                        "",
                        '$.commonUtils.focus("password-old");'
                    );
                    return false;
                }
                if ($("#password-old").val() === $("#password-new").val()) {
                    $.commonUtils.alertWithFn(
                        ka.msg.mypage.passwordOldEqual,
                        "",
                        '$.commonUtils.focus("password-new");'
                    );
                    return false;
                }
                if (
                    $.joinUtils.checkPassword("password-new") &&
                    $.joinUtils.checkPassword("password-new-confirm")
                ) {
                    $.commonUtils.confirm(
                        ka.msg.mypage.password,
                        ka.msg.mypage.passwordConfirm,
                        "submitPassword()"
                    );
                }
            },

            /*------------------------------------------------------------------
              * @function:    validatePasswordSubmit
              * @description: 정보수정 - 비밀번호 수정 서버 처리
              ------------------------------------------------------------------*/
            validatePasswordSubmit: function (param) {
                var result = $.ajaxUtils.getApiData(
                    "/api/Member/ChangePassword",
                    param,
                    null,
                    false
                );
                if ($.ajaxUtils.getResultCode(result) === "00") {
                    $.commonUtils.alert(ka.msg.mypage.passwordOk, "success");
                    $("#password-old").val("");
                    $("#password-new").val("");
                    $("#password-new-error").remove();
                    $("#password-new-confirm").val("");
                    $("#password-new-confirm-error").remove();
                } else {
                    $.commonUtils.alert(eval(result.code));
                }
            },

            /*------------------------------------------------------------------
              * @function:    validateRetire
              * @description: 탈퇴 하기 클릭 시 입력값 체크
              ------------------------------------------------------------------*/
            validateRetire: function () {
                var retireOption = new Array();
                $.each($("input[name='retire-option']"), function (index, item) {
                    if (item.checked) retireOption.push(item.value);
                });
                $("#retire-option").val(retireOption.join("^"));

                if (
                    $("#retire-reason").val() === "" &&
                    (retireOption.length === 0 || $("#retire-option").val() === "")
                ) {
                    $.commonUtils.alert(ka.msg.mypage.retireOption);
                    return false;
                }

                if (!$("input:checkbox[id='retire-agreement']").is(":checked")) {
                    $.commonUtils.alert(ka.msg.mypage.retireAgree);
                    return false;
                }

                $.commonUtils.confirm(
                    ka.msg.mypage.retire,
                    ka.msg.mypage.retireConfirm,
                    "$.mypageUtils.retireProcess();"
                );

                return false;
            },

            validateRetireV2: function () {
                var retireOption = new Array();
                $.each($("input[name='delete_account_check']"), function (index, item) {
                    if (item.checked) retireOption.push(item.value);
                });
                $("#retire-option").val(retireOption.join("^"));

                if (
                    $("#retire-reason").val() === "" &&
                    (retireOption.length === 0 || $("#retire-option").val() === "")
                ) {
                    $.commonUtils.alert(ka.msg.mypage.retireOption);
                    return false;
                }

                if ($("#input_id").val().replace(/ /gi, "") === "") {
                    $.commonUtils.alert(ka.msg.join.idEmpty);
                    return false;
                }

                if ($("#input_pwd").val().replace(/ /gi, "") === "") {
                    $.commonUtils.alert(ka.msg.join.pwdEmpty);
                    return false;
                }

                if (!$("input:checkbox[id='delete_account_agree']").is(":checked")) {
                    $.commonUtils.alert(ka.msg.mypage.retireAgree);
                    return false;
                }

                $.commonUtils.confirm(
                    ka.msg.mypage.retire,
                    ka.msg.mypage.retireConfirm,
                    "$.mypageUtils.retireProcess();"
                );

                return false;
            },

            /*------------------------------------------------------------------
              * @function:    retireProcess
              * @description: 탈퇴 묻는 창에서 예 클릭 시 서버 Submit 처리
              ------------------------------------------------------------------*/
            retireProcess: async function () {
                var param = {};
                param["retire_option"] = "";
                $('input:checkbox[name="delete_account_check"]').each(function () {
                    if (this.checked) {
                        if (param["retire_option"] !== "") param["retire_option"] += "^";
                        param["retire_option"] += this.value;
                    }
                });
                param["retire_reason"] = $("#retire-reason").val();
                param["input_id"] = $("#input_id").val();
                param["input_pwd"] = $("#input_pwd").val();

                $("#modal-warning").modal("hide");

                const $submitBtn = document.querySelector('#retire-member-btn');
                $submitBtn.setAttribute('disabled', 'true');
                const tempContent = $submitBtn.innerHTML;
                $submitBtn.innerHTML = `
                        <i class='fa fa-spinner fa-spin'></i>
                        Loading
                    `;
                const response = await fetch('/api/Member/SetRetire', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(param),
                });
                
                const result = await response.json();

                $submitBtn.removeAttribute('disabled');
                $submitBtn.innerHTML = tempContent;
                
                const resultCode = $.ajaxUtils.getResultCode(result);
                if (resultCode === "00") {
                    $.commonUtils.redirectAlert(
                        ka.msg.mypage.retireComplete,
                        "success",
                        "/Member/Logout"
                    );
                } else if (resultCode === "201") {
                    $.commonUtils.alert(result.message);
                } else {
                    $("#modal-retire-error").addClass("block");
                    $("body").addClass("yhidden");
                    $(".modal-bg").addClass("block");
                    $.commonUtils.alert(ka.msg.common.error);
                    console.error(result);
                }
            },

            /*------------------------------------------------------------------
              * @function:    validateInfo
              * @description: 정보수정 - 회원정보 수정 클릭 시 입력 값 유효성 체크
              ------------------------------------------------------------------*/
            validateInfo: function () {
                var utils = $.mypageUtils;
                var joinType = $("#join-type").val();
                var target = parseInt(joinType, 10) - 1;

                // 법인정보 수정 체크
                var changeCorp = $("#ckbChangeCorp").is(":checked");
                //if (joinType === "003" || joinType === "004") {
                //    if (!changeCorp
                //        && ($("#company-reg-doc-file-" + (target + 1)).val() !== ""
                //            || $("#company-business-card-file-" + (target + 1)).val() !== "")
                //        || $("#company-name-" + (target + 1)).val() !== $("#join-company-name").val()
                //        || $("#company-president-" + (target + 1)).val() !== $("#join-company-president").val()
                //        || $("#company-rep-tel-" + (target + 1)).val() !== $("#join-company-rep-tel").val()
                //        || $("#company-reg-no-" + (target + 1)).val() !== $("#join-company-reg-no").val()
                //    ) {
                //        $.commonUtils.alert(ka.msg.mypage.corporateChange);
                //        return false;
                //    }
                //}
                if ((joinType === "003" || joinType === "004") && changeCorp) {
                    if (
                        $("#company-reg-doc-file-" + (target + 1)).val() === "" ||
                        $("#company-business-card-file-" + (target + 1)).val() === ""
                    ) {
                        $.commonUtils.alert(ka.msg.mypage.corporateChange);
                        return false;
                    }
                }

                utils.requestParam = {};
                utils.requestParam["type"] = joinType;
                utils.requestParam["mobile_original"] = $(
                    "#join-mobile-original"
                ).val();
                utils.requestParam["identification-original"] = $(
                    "#join-identification-original"
                ).val();
                utils.requestParam["info_validate_period"] = $(
                    "#join-info-validate-period"
                ).val();
                utils.requestParam["receive_sms_info"] = $("#ad-chkbox_01").is(
                    ":checked"
                )
                    ? "Y"
                    : "N";
                utils.requestParam["receive_email_info"] = $("#ad-chkbox_02").is(
                    ":checked"
                )
                    ? "Y"
                    : "N";
                utils.requestParam["receive_phone_info"] = $("#ad-chkbox_03").is(
                    ":checked"
                )
                    ? "Y"
                    : "N";

                utils.modifyMsg = "";
                utils.fields[target].forEach(function (item) {
                    if (
                        $("#fg-" + item + "-" + (target + 1))
                            .get(0)
                            .className.indexOf("required") > 0 &&
                        $("#" + item + "-" + (target + 1)).val() === ""
                    ) {
                        utils.modifyMsg = $.commonUtils.alert(
                            $("#" + item + "-" + (target + 1)).get(0).dataset.msg
                        );
                    } else {
                        utils.requestParam[item.replace(/-/gi, "_")] = $(
                            "#" + item + "-" + (target + 1)
                        ).val();
                    }
                });

                if ($.joinUtils.authFlag[target]) {
                    utils.requestParam["mobile"] = $("#mobile-" + (target + 1)).val();
                } else {
                    utils.requestParam["mobile"] = $("#join-mobile-original").val();
                }

                // 첨부 파일 처리
                if (joinType === "002") {
                    if ($("#identification-file-" + (target + 1)).val() !== "") {
                        var uploadResult = $.ajaxUtils.getUploadData(
                            "identification-file-" + (target + 1),
                            "/api/File/Upload/Identification"
                        );
                        if (uploadResult["result"]) {
                            utils.requestParam["identification"] =
                                uploadResult.file_info.filename;
                            utils.requestParam["info_req_file"] = "Y";
                        } else {
                            $.commonUtils.alert(ka.msg.common.fileError);
                            return false;
                        }
                    } else {
                        utils.requestParam["identification_original"] = $(
                            "#join-identification-original"
                        ).val();
                    }
                } else if (joinType === "003" || joinType === "004") {
                    if ($("#company-reg-doc-file-" + (target + 1)).val() !== "") {
                        var uploadResult = $.ajaxUtils.getUploadData(
                            "company-reg-doc-file-" + (target + 1),
                            "/api/File/Upload/CompanyRegDoc"
                        );
                        if (uploadResult["result"]) {
                            utils.requestParam["company_reg_doc"] =
                                uploadResult.file_info.filename;
                            utils.requestParam["info_req_file"] = "Y";
                        } else {
                            $.commonUtils.alert(ka.msg.common.fileError);
                            return false;
                        }
                    } else {
                        utils.requestParam["company_reg_doc_original"] = $(
                            "#join-company-reg-doc-original"
                        ).val();
                    }

                    if ($("#company-business-card-file-" + (target + 1)).val() !== "") {
                        var uploadResult = $.ajaxUtils.getUploadData(
                            "company-business-card-file-" + (target + 1),
                            "/api/File/Upload/CompanyBusinessCard"
                        );
                        if (uploadResult["result"]) {
                            utils.requestParam["company_business_card"] =
                                uploadResult.file_info.filename;
                        } else {
                            $.commonUtils.alert(ka.msg.common.fileError);
                            return false;
                        }
                    } else {
                        utils.requestParam["company_business_card_original"] = $(
                            "#join-company-business-card-original"
                        ).val();
                    }
                }

                // 모바일/신용카드 인증 SEQ
                utils.requestParam["mobile_auth_seq"] = $(
                    "#join-mobile-auth-seq"
                ).val();

                if (utils.modifyMsg === "") {
                    $.commonUtils.confirm(
                        ka.msg.common.modify,
                        ka.msg.mypage.infoConfirm,
                        "$.mypageUtils.validateInfoSubmit();"
                    );
                    return false;
                } else {
                    $.commonUtils.alert(utils.modifyMsg);
                    return false;
                }
            },

            validateInfoSubmit: function () {
                var result = $.ajaxUtils.getApiData(
                    "/api/Member/SetMember",
                    $.mypageUtils.requestParam,
                    null,
                    false
                );
                if ($.ajaxUtils.getResultCode(result) === "00") {
                    $.commonUtils.alert(ka.msg.mypage.infoOk, result.code);
                } else {
                    $.commonUtils.alert(ka.msg.common.error);
                }
            },

            openAddressPopup: function (type) {
                $.mypageUtils.selectedUid = 0;
                $("input:radio[name=modal-type]:input[value=001]").attr(
                    "checked",
                    true
                );
                $.mypageUtils.addressFields.forEach(function (item) {
                    $("#" + item).val("");
                });

                if (type === "U") {
                    $.mypageUtils.selectedUid = $(
                        "input:radio[name=address_select]:checked"
                    ).val();
                    if ($.mypageUtils.selectedUid) {
                        var row = $("#tr_" + $.mypageUtils.selectedUid.toString());
                        $.each(row.get(0).dataset, function (key, value) {
                            if (key === "type") {
                                const input = $(`input:radio[name=modal-type]:input[value=${value}]`);
                                input.attr("checked", true);
                                input.click();
                            } else if (key === "countryCode") {
                                if (value && value !== 'null') {
                                    $("#modal-country-code").val(value).trigger("change");
                                } else {
                                    $("#modal-country-code").val("KOR").trigger("change");
                                }
                            } else if (key === "zipcode") {
                                $("#modal-zip-code").val(value);
                            } else {
                                value = key === "contact" ? value.replace(/-/g, "") : value;
                                $("#modal-" + key).val(value);
                            }
                        });
                    } else {
                        return false;
                    }
                } else if (type === "I") {
                    if (typeof memName === "string") {
                        $("#modal-receiver").val($.commonUtils.decodeHTML(memName));
                        $("#modal-contact").val(memMobile.replace(/-/g, ""));
                        $("#modal-country-code").val("KOR").trigger("change");
                    }
                }

                if (type === "I" || type === "U") {
                    $.mypageUtils.procType = type;
                    $("#modal-address").modal("show");
                    $("#modal-address-btn").html(
                        type === "U" ? ka.msg.common.modify : ka.msg.common.complete
                    );
                }
            },

            /*------------------------------------------------------------------
              * @function:    checkAddressType
              * @param:       obj (주소 유형 input 개체)
              * @description: 직접 입력인 경우 입력 필드 활성화 처리
              ------------------------------------------------------------------*/
            checkAddressType: function (obj) {
                if (obj.value === "004") {
                    $("#fg-modal-etc").focus();
                    $("#fg-modal-etc").show();
                } else {
                    $("#fg-modal-etc").hide();
                }
            },

            /*------------------------------------------------------------------
              * @function:    checkCountry
              * @param:       obj (국가 select 개체)
              * @description: 대한민국이 아닌 경우 주소 직접 입력 처리
              ------------------------------------------------------------------*/
            checkCountry: function (obj) {
                if (obj.value === "KOR") {
                    $("#modal-search-btn").show();
                    $("#fg-modal-zipcode").addClass("disabled");
                    $("#modal-zipcode").attr("disabled", true);
                    $("#fg-modal-zip-code").addClass("disabled");
                    $("#modal-zip-code").attr("disabled", true);
                    $("#fg-modal-address1").addClass("disabled");
                    $("#modal-address1").attr("disabled", true);
                } else {
                    $("#modal-search-btn").hide();
                    $("#fg-modal-zipcode").removeClass("disabled");
                    $("#modal-zipcode").attr("disabled", false);
                    $("#fg-modal-zip-code").removeClass("disabled");
                    $("#modal-zip-code").attr("disabled", false);
                    $("#fg-modal-address1").removeClass("disabled");
                    $("#modal-address1").attr("disabled", false);
                    $.joinUtils.hideZipCodeLayer();
                }
                $("#modal-address1").val("");
                $("#modal-address2").val("");
                $("#modal-zipcode").val("");
            },

            /*------------------------------------------------------------------
              * @function:    getApiResult
              * @description: 주소관리 - 배송지 추가/수정 처리
              ------------------------------------------------------------------*/
            updateAddressResult: async function () {
                if ($.mypageUtils.process) return false;

                $.mypageUtils.process = true;

                var error = false;
                var param = {};
                param["mode"] = $.mypageUtils.procType === "I" ? "INSERT" : "UPDATE";
                param["type"] = $("input:radio[name=modal-type]:checked").val();
                param["uid"] = $.mypageUtils.selectedUid;
                $.mypageUtils.addressFields.forEach(function (item) {
                    if ($("#" + item).val() === "") {
                        $.joinUtils.validateMsg(item, $("#" + item).attr("data-msg"));
                        error = true;
                    } else if (item.replace(/-/gi, "").indexOf("zipcode") > -1) {
                        if ($("#modal-zipcode").length > 0) {
                            param["zipcode"] = $("#modal-zipcode").val();
                        } else {
                            param["zipcode"] = $("#" + item).val();
                        }
                        $("#" + item + "-error").remove();
                    } else {
                        param[
                            item
                                .replace("modal-", "")
                                .replace("zip-code", "zipcode")
                                .replace(/-/gi, "_")
                        ] = $("#" + item).val();
                        $("#" + item + "-error").remove();
                    }
                });

                if (param["type"] === "004") {
                    if ($("#modal-etc").val() === "") {
                        $.joinUtils.validateMsg(item, $("#modal-etc").attr("data-msg"));
                        error = true;
                    } else {
                        param["etc"] = $("#modal-etc").val();
                    }
                }

                if (
                    param["mode"] === "INSERT" &&
                    $("#address_add_info_all_agree").length > 0
                ) {
                    param["mode"] = "INSERT_AGREE";
                    param["collect_personal_info_address"] = "Y";
                    param["provide_personal_info_agree"] = "Y";
                }

                if (!error) {
                    param["mode"] =
                        location.href.toLowerCase().indexOf("/bidapplication") > 0 ||
                            location.href.toLowerCase().indexOf("/liverequest") > 0 ||
                            location.href.toLowerCase().indexOf("/auction/major") > 0 ||
                            location.href.toLowerCase().indexOf("/auction/premium") > 0 ||
                            location.href.toLowerCase().indexOf("/auction/weekly") > 0 ||
                            location.href.toLowerCase().indexOf("/member/joinstepcomplete") > 0
                            ? "INSERT_BID"
                            : param["mode"];
                    const $submitBtn = document.querySelector('#address-modal__submit-btn');
                    const $submitContent = document.querySelector('#modal-address-btn');
                    $submitBtn.setAttribute('disabled', 'true');
                    const tempContent = $submitContent.innerHTML;
                    $submitContent.innerHTML = `
                        <i class='fa fa-spinner fa-spin'></i>
                        Loading
                    `;

                    const response = await fetch('/api/Member/AddressInfo', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                        },
                        body: JSON.stringify(param),
                    });
                    const result = await response.json();

                    $submitBtn.removeAttribute('disabled');
                    $submitContent.innerHTML = tempContent;
                    
                    try {
                        if (result.code === "00") {
                            if (param["mode"] === "INSERT_AGREE") {
                                location.reload();
                            } else if (param["mode"] === "INSERT_BID") {
                                $.commonUtils.alertWithFn(
                                    ka.msg.common.saveComplete,
                                    "success",
                                    "location.reload();"
                                );
                            } else {
                                if (typeof getAddressList === "function") {
                                    getAddressList();
                                }
                            }
                        } else {
                            $.commonUtils.alert(ka.msg.common.error);
                        }
                    } catch (e) {
                        console.log(e.description);
                    }
                    $("#modal-address").modal("hide");
                } else {
                    $.commonUtils.alert(ka.msg.common.error);
                }

                $.mypageUtils.process = false;
            },

            setPrimary: function () {
                if ($.mypageUtils.process) return false;

                $.mypageUtils.process = true;

                var param = {};
                param["mode"] = "PRIMARY";
                param["uid"] = $("input:radio[name=address_select]:checked").val();
                if (param["uid"]) {
                    var result = $.ajaxUtils.getApiData(
                        "/api/Member/AddressInfo",
                        param,
                        null,
                        false
                    );
                    try {
                        if (result.code === "00") {
                            $.commonUtils.alert(ka.msg.mypage.addressDefault, "success");
                            getAddressList();
                        } else {
                            $.commonUtils.alert(ka.msg.common.error);
                        }
                    } catch (e) {
                        console.log(e.description);
                    }
                }
                $.mypageUtils.process = false;
            },

            deleteConfirm: function () {
                if ($("input:radio[name=address_select]:checked").length > 0) {
                    $.commonUtils.confirm(
                        ka.msg.common.delete,
                        ka.msg.common.deleteConfirm,
                        "$.mypageUtils.delete();"
                    );
                }
            },

            delete: function () {
                var param = {};
                param["mode"] = "DELETE";
                param["uid"] = $("input:radio[name=address_select]:checked").val();

                if (
                    $("#tr_" + param["uid"]).get(0).dataset["primary"] &&
                    $("#tr_" + param["uid"]).get(0).dataset["primary"] === "Y"
                ) {
                    $.commonUtils.alert(ka.msg.mypage.defaultAddressDelete);
                } else if (
                    $("#tr_" + param["uid"]).get(0).dataset["applyBook"] &&
                    $("#tr_" + param["uid"]).get(0).dataset["applyBook"] === "Y"
                ) {
                    $.commonUtils.alert(ka.msg.mypage.applyBookAddressDelete);
                } else {
                    var result = $.ajaxUtils.getApiData(
                        "/api/Member/AddressInfo",
                        param,
                        null,
                        false
                    );
                    try {
                        if (result.code === "00") {
                            getAddressList();
                        } else {
                            $.commonUtils.alert(ka.msg.common.error);
                        }
                    } catch (e) {
                        console.log(e.description);
                    }
                }
            },

            openBidList: function (workSeq) {
                if ($.loginUtils.isLogin()) {
                    $.ajaxUtils.getApiData(
                        "/api/Auction/BidList/" + workSeq,
                        null,
                        $.mypageUtils.getBidListComplete
                    );
                } else {
                    $.commonUtils.openLogin(ka.msg.auction.bidListCheck);
                }
            },

            openMyBidList: function (workSeq) {
                if ($.loginUtils.isLogin()) {
                    $.ajaxUtils.getApiData(
                        "/api/Auction/MyBidList/" + workSeq,
                        null,
                        $.mypageUtils.getBidListComplete
                    );
                } else {
                    $.commonUtils.openLogin(ka.msg.auction.bidListCheck);
                }
            },

            getBidListComplete: function (result) {
                if ($.ajaxUtils.getResultCode(result) === "00") {
                    $("#tbl-bid-body").empty();
                    var count = -1;
                    $.each(result.data, function (index, item) {
                        if (index < $.auction.bidListSize) {
                            var etc =
                                item.bid_type === "002" ? ka.msg.auction.automaticBidding : "";
                            etc +=
                                item.nak_yn === "Y"
                                    ? etc !== ""
                                        ? "<br />" + ka.msg.auction.winningBid
                                        : ka.msg.auction.winningBid
                                    : "";
                            var tr = $("<tr />");
                            tr.append($("<td class='text-center' />").append(item.mem_id));

                            // 2022.02.23 :: 최고가 표시
                            let pricePrefix =
                                item.bid_uid === item.highest_uid
                                    ? "<span style='vertical-align: text-bottom;'>" +
                                    ka.icon.highest +
                                    "</span>"
                                    : "";
                            let displayPrice =
                                typeof isKor === "string" && isKor === "False"
                                    ? "KRW " + $.stringUtils.comma(item.price_bid)
                                    : $.stringUtils.comma(item.price_bid) + "원";
                            displayPrice =
                                pricePrefix === "" ? displayPrice : pricePrefix + displayPrice;
                            tr.append($("<td class='text-center' />").append(displayPrice));

                            tr.append(
                                $("<td class='text-center' />", { class: "date" }).append(
                                    item.reg_ymd + " " + item.reg_hms
                                )
                            );
                            tr.append(
                                $("<td class='text-center text-primary' />").append(etc)
                            );
                            $("#tbl-bid-body").append(tr);
                        }
                        count = index++;
                    });

                    if (count === -1) {
                        $("#tbl-bid-body").append(
                            $("<tr />").append(
                                $("<td />", {
                                    colspan: "5",
                                    style: "text-align: center;",
                                }).append(ka.msg.list.emptyBid)
                            )
                        );
                    }

                    $("#cur-sit").show();
                    $(".cursit-bg").show();
                }
            },

            setApplyBook: function () {
                $("#apply_book_btn_wrap").hide();

                $("#apply-book-name-msg").html("");
                $("#apply-book-mobile-msg").html("");
                $("#apply_book_agree_msg").html("");

                if ($("#apply-book-name").val().replace(/ /gi, "") === "") {
                    $("#apply-book-name-msg").html("구독자 이름을 입력하세요.");
                    $("#apply-book-name").focus();
                    return;
                }

                if ($("#apply-book-mobile").val().replace(/ /gi, "") === "") {
                    $("#apply-book-mobile-msg").html("구독자 연락처를 입력하세요.");
                    $("#apply-book-mobile").focus();
                    return;
                }

                if ($("#apply_book_agree").is(":checked")) {
                    let el = $("#apply-book-address option:selected");
                    let reqParam = {};
                    reqParam["address_uid"] = el.get(0).dataset.uid;
                    reqParam["receiver"] = $("#apply-book-name").val();
                    reqParam["contact"] = $("#apply-book-mobile").val();
                    //let result = $.ajaxUtils.getApiData("/api/MyPage/SetApplyBook", reqParam, null, false);
                    //if (result.code === "00") {
                    //    location.reload();
                    //    // $("#modal-common").modal('hide');
                    //} else {
                    //    $("#apply_book_agree_msg").html(eval(result.code));
                    //}
                    $.ajaxUtils.getApiData(
                        "/api/MyPage/SetApplyBook",
                        reqParam,
                        $.mypageUtils.setApplyBookComplete
                    );
                } else {
                    $("#apply_book_agree_msg").html(
                        "개인정보 제3자 제공 동의를 선택하세요."
                    );
                }

                $("#apply_book_btn_wrap").show();
            },

            setApplyBookComplete: function (result) {
                if ($.ajaxUtils.getResultCode(result) === "00") {
                    if (result.code === "00") {
                        $(".applyBook_content").hide();
                        $(".applyBook_content_complete").show();
                        if (applyKind === "") {
                            if (typeof applyKind === "string") {
                                applyKind = "010";
                            }
                        } else {
                            $("#applyBookCompleteTitle").html("정보가 수정되었습니다.");
                            $("#applyBookCompleteContent").remove();
                        }
                    } else {
                        $("#apply_book_agree_msg").html(eval(result.code));
                    }
                }
            },

            setCertificateService: function () {
                if ($.mypageUtils.process) return false;

                $("#certificate_agree_msg").html("");

                let certificateAddress = $("#certificate-address option:selected");
                let certificateReceiverName = $("#certificate-receiver-name").val();
                let certificateReceiverMobile = $("#certificate-receiver-mobile").val();
                let certificateReceiverEmail = $("#certificate-receiver-email").val();
                let certificateInfoAgree = $("#certificate-info-agree:checked").val();

                let paramCertificateRequest = {};
                paramCertificateRequest["address_uid"] =
                    certificateAddress.get(0).dataset.uid;
                paramCertificateRequest["work_uid"] = $("#certificate-work-uid").val();
                paramCertificateRequest["email_flag"] =
                    typeof certificateInfoAgree === "undefinee"
                        ? "N"
                        : certificateInfoAgree;

                if (certificateReceiverName.replace(/ /gi, "") === "") {
                    $("#certificate_agree_msg").html("수신자 이름을 입력하세요.");
                    return false;
                } else {
                    paramCertificateRequest["receiver_name"] = certificateReceiverName;
                }

                if (certificateReceiverMobile.replace(/ /gi, "") === "") {
                    $("#certificate_agree_msg").html("수신자 연락처를 입력하세요.");
                    return false;
                } else {
                    paramCertificateRequest["receiver_mobile"] =
                        certificateReceiverMobile;
                }

                if (certificateReceiverEmail.replace(/ /gi, "") === "") {
                    $("#certificate_agree_msg").html("수신자 이메일을 입력하세요.");
                    return false;
                } else {
                    paramCertificateRequest["receiver_email"] = certificateReceiverEmail;
                }
                $.mypageUtils.process = true;
                $.ajaxUtils.getApiData(
                    "/api/MyPage/SetCertificateRequest",
                    paramCertificateRequest,
                    $.mypageUtils.setCertificateServiceComplete
                );
            },

            setCertificateServiceComplete(result) {
                if ($.ajaxUtils.getResultCode(result) === "00") {
                    $(".applyBook_content").hide();
                    $(".applyBook_content_complete").show();
                    $("#applyBookCompleteContent").remove();
                    $("#applyBookCompleteTitle").html("신청하였습니다.");
                } else {
                    $("#applyBookCompleteTitle").html(ka.msg.common.error);
                }
                $.mypageUtils.process = false;
            },

            setMember: function () {
                $.commonUtils.confirm(ka.msg.common.save, ka.msg.common.saveConfirm, "$.mypageUtils.setMemberProc();" );
            },

            setMemberProc: function () {
                var memParam = {};
                // 기본정보
                memParam["type"] = $("#join-type").val();
                memParam["name"] = $("#join-name").val();
                memParam["sex"] = $("#sex").val();
                memParam["birth_date"] = $("#join-birth-date").val();
                
                if (!$.validateUtils.checkEmail("email")) {
                    $.commonUtils.alert(ka.msg.join.memEmailRule);
                    return false;
                }                
                memParam["email"] = $("#email").val();

                // 기본정보 (법인)
                if (memParam["type"] === "003") {
                    if ($("#company_name").val().replace(/ /gi, "") === "") {
                        $.commonUtils.alert(ka.msg.mypage.companyNameEmpty);
                        return false;
                    }
                    if ($("#company_president").val().replace(/ /gi, "") === "") {
                        $.commonUtils.alert(ka.msg.mypage.companyPresidentEmpty);
                        return false;
                    }
                    if ($("#company_rep_tel").val().replace(/ /gi, "") === "") {
                        $.commonUtils.alert(ka.msg.mypage.companyRepTelEmpty);
                        return false;
                    }
                    if ($("#tax_email").val().replace(/ /gi, "") === "") {
                        $.commonUtils.alert(ka.msg.mypage.taxEmailEmpty);
                        return false;
                    }

                    if (!$.validateUtils.checkEmail("email")) {
                        $.commonUtils.alert(ka.msg.join.memEmailRule);
                        return false;
                    }

                    if (!$.validateUtils.checkEmail("tax_email")) {
                        $.commonUtils.alert(ka.msg.join.memEmailRule);
                        return false;
                    }

                    memParam["company_reg_no"] = $("#join-company-reg-no").val();
                    memParam["company_name"] = $("#company_name").val();
                    memParam["company_president"] = $("#company_president").val();
                    memParam["company_rep_tel"] = $("#company_rep_tel").val();
                    memParam["tax_email"] = $("#tax_email").val();

                    memParam["name"] = $("#name").val();
                }

                // 인증정보
                memParam["mobile_seq"] = $("#join-mobile-seq").val();

                // 추가정보
                memParam["job_code"] = $("#job option:selected").val();
                memParam["company_name"] = $("#company_name").val();
                memParam["company_tel"] = $("#company_tel").val();
                memParam["company_fax"] = $("#company_fax").val();

                if (memParam["type"] === "002") {
                    if ($("#identification-file").val() !== "") {
                        var uploadResult = $.ajaxUtils.getUploadData(
                            "identification-file",
                            "/api/File/Upload/Identification"
                        );
                        if (uploadResult["result"]) {
                            memParam["identification"] = uploadResult.file_info.filename;
                            memParam["info_req_file"] = "Y";
                        } else {
                            $.commonUtils.alert(ka.msg.common.fileError);
                            return false;
                        }
                    }
                } else if (memParam["type"] === "003" || memParam["type"] === "004") {
                    if (
                        $("#company_reg_doc_file").length > 0 &&
                        $("#company_reg_doc_file").val() !== ""
                    ) {
                        var uploadResult = $.ajaxUtils.getUploadData(
                            "company-reg-doc-file",
                            "/api/File/Upload/CompanyRegDoc"
                        );
                        if (uploadResult["result"]) {
                            memParam["company_reg_doc"] = uploadResult.file_info.filename;
                            memParam["info_req_file"] = "Y";
                        } else {
                            $.commonUtils.alert(ka.msg.common.fileError);
                            return false;
                        }
                    }
                    if (
                        $("#company_business_card_file").length > 0 &&
                        $("#company_business_card_file").val() !== ""
                    ) {
                        var uploadResult = $.ajaxUtils.getUploadData(
                            "company-business-card-file",
                            "/api/File/Upload/CompanyBusinessCard"
                        );
                        if (uploadResult["result"]) {
                            memParam["company_business_card"] =
                                uploadResult.file_info.filename;
                        } else {
                            $.commonUtils.alert(ka.msg.common.fileError);
                            return false;
                        }
                    }
                }

                var result = $.ajaxUtils.getApiData(
                    "/api/MyPage/SetMember",
                    memParam,
                    null,
                    false
                );
                if (result.code === "00") {
                    $.commonUtils.alertWithFn(
                        ka.msg.common.saveComplete,
                        "success",
                        "location.reload();"
                    );
                } else {
                    $.commonUtils.alert(eval(result.code));
                }
            },

            setAccount: function () {
                $.commonUtils.confirm(
                    ka.msg.common.save,
                    ka.msg.common.saveConfirm,
                    "$.mypageUtils.setAccountProc();"
                );
            },

            setAccountProc: function () {
                var accountParam = {};
                // 휴면 전환 기준
                accountParam["info_validate_period"] = $(
                    "input:radio[name=Dormant_check]:checked"
                ).val();
                // 메인 하이라이트
                accountParam["main_highlight"] = $(
                    "input:radio[name=main_check]:checked"
                ).val();
                // LOT 목록
                accountParam["list_view_mode"] = $(
                    "input:radio[name=lot_list_check]:checked"
                ).val();
                // 광고성 정보 수신 동의
                accountParam["receive_sms_info"] = $("#mypageAdd_agree_check_01").is(
                    ":checked"
                )
                    ? "Y"
                    : "N";
                accountParam["receive_email_info"] = $("#mypageAdd_agree_check_02").is(
                    ":checked"
                )
                    ? "Y"
                    : "N";
                accountParam["receive_phone_info"] = $("#mypageAdd_agree_check_03").is(
                    ":checked"
                )
                    ? "Y"
                    : "N";

                var result = $.ajaxUtils.getApiData(
                    "/api/MyPage/SetAccount",
                    accountParam,
                    null,
                    false
                );
                if (result.code === "00") {
                    $.commonUtils.alertWithFn(ka.msg.common.saveComplete, "success", "window.location.reload();");
                } else {
                    $.commonUtils.alert(eval(result.code));
                }
            },

            checkPassword: async function () {
                var passwordOld = $("#password-old").val();
                if (passwordOld.replace(/ /gi, "") === "") {
                    $.commonUtils.alert(ka.msg.mypage.passwordEmpty);
                    return false;
                }

                var pwdParam = {};
                pwdParam["password_old"] = passwordOld;
                
                const $submitBtn = document.querySelector('#btn-password-old');
                $submitBtn.setAttribute('disabled', 'true');
                const tempContent = $submitBtn.innerHTML;
                $submitBtn.innerHTML = `
                        <i class='fa fa-spinner fa-spin'></i>
                        Loading
                    `;
                
                const response = await fetch('/api/MyPage/CheckPassword', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(pwdParam),
                });

                const result = await response.json();
                $submitBtn.removeAttribute('disabled');
                $submitBtn.innerHTML = tempContent;
                
                if (result.code === "00") {
                    $("#password-old").attr("disabled", "disabled");
                    $("#btn-password-old").attr("disabled", "disabled");
                    $submitBtn.setAttribute('disabled', 'disabled');
                } else {
                    $.commonUtils.alert(eval(result.code));
                }
            },

            changePassword: function () {
                var passwordOld = $("#password-old").val();

                if (
                    !(
                        $("#password-old").attr("disabled") &&
                        $("#password-old").attr("disabled") === "disabled"
                    )
                ) {
                    $.commonUtils.alert(ka.msg.mypage.passwordCheck);
                    return false;
                }

                if (passwordOld.replace(/ /gi, "") === "") {
                    $.commonUtils.alert(ka.msg.mypage.passwordEmpty);
                    return false;
                }

                if ($("#password-old").val() === $("#password-new").val()) {
                    $.commonUtils.alertWithFn(
                        ka.msg.mypage.passwordOldEqual,
                        "",
                        '$.commonUtils.focus("password-new");'
                    );
                    return false;
                }

                if ($("#password-new").val() !== $("#password-new-confirm").val()) {
                    $.commonUtils.alertWithFn(
                        ka.msg.mypage.passwordNotMatch,
                        "",
                        '$.commonUtils.focus("password-new");'
                    );
                    return false;
                }

                if (
                    !$.joinUtils.checkPassword("password-new") ||
                    !$.joinUtils.checkPassword("password-new-confirm")
                ) {
                    $.commonUtils.alert(ka.msg.join.memPwdRuleLength);
                    return false;
                }

                if (
                    $.joinUtils.checkPassword("password-new") &&
                    $.joinUtils.checkPassword("password-new-confirm")
                ) {
                    $.commonUtils.confirm(
                        ka.msg.mypage.password,
                        ka.msg.mypage.passwordConfirm,
                        "submitPassword()"
                    );
                }
            },

            changePasswordProc: function () {
                var pwdParam = {};
                pwdParam["password_old"] = $("#password-old").val();
                pwdParam["password_new"] = $("#password-new").val();
                pwdParam["password_new_confirm"] = $("#password-new-confirm").val();

                var result = $.ajaxUtils.getApiData(
                    "/api/Member/ChangePassword",
                    pwdParam,
                    null,
                    false
                );
                if (result.code === "00") {
                    $.commonUtils.alert(ka.msg.mypage.passwordOk, "success");
                    $(".mypage_modal_close").click();
                } else {
                    $.commonUtils.alert(eval(result.code));
                }
            },

            mobileValidation: function (mode, type) {                
                var paramValidation = {};
                var result;
                if (mode === "R") {
                    // 요청
                    let number;
                    if (type === '002' || type === '004') {
                        let iti = intlTelInput(document.querySelector(".country_tel"));
                        number = iti.getNumber();
                    } else {
                        number = document.querySelector("#usernumber").value;
                    }
                    if (number.replace(/ /gi, "") === "") {
                        $("#member_input_message").html(ka.msg.join.memMobileRuleEmpty);
                        return false;
                    }

                    paramValidation["number"] = number;
                    paramValidation["dial_code"] = (type === '002' || type === '004') ? $(".iti__selected-dial-code").html() : '';

                    result = $.ajaxUtils.getApiData(
                        "/api/MyPage/GetMobileValidationCode",
                        paramValidation,
                        null,
                        false
                    );

                    if (result.code === "00") {
                        $.mypageUtils.mobileValidationState = "R";

                        // 버튼 처리
                        $("#btn-validation-confirm").removeClass("disabled");
                        $("#btn-validation-confirm").removeAttr("disabled");
                        $("#btn-validation-re-request").addClass("disabled");
                        $("#btn-validation-request").hide();
                        $("#btn-validation-cancel").show();

                        // 입력필드 처리
                        $("#usernumber").addClass("disabled");
                        $("#usernumber").attr("disabled", "disabled");
                        $("#usercode").removeAttr("disabled");

                        if (
                            location.href.indexOf("www.dev.") > -1 ||
                            location.href.indexOf("www.release.") > -1 ||
                            location.href.indexOf("localhost") > -1
                        ) {
                            $("#member_input_message").html(
                                "테스트계는 인증번호를 제공 합니다.<br />인증번호 : " +
                                result.data
                            );
                        }
                        $.mypageUtils.authSeq = result.message;
                    } else {
                        $.commonUtils.alert(eval(result.code));
                        $.mypageUtils.mobileValidationState = "";
                    }
                } else if (mode === "A") {
                    // 인증확인
                    if ($.mypageUtils.mobileValidationState === "R") {
                        if ($("#usercode").val().replace(/ /gi, "") === "") {
                            $("#member_input_message").html(ka.msg.join.reqCodeEmpty);
                            return false;
                        }
                        paramValidation["auth_seq"] = $.mypageUtils.authSeq;
                        paramValidation["auth_code"] = $("#usercode").val();
                        result = $.ajaxUtils.getApiData(
                            "/api/MyPage/SetMobileValidationCode",
                            paramValidation,
                            null,
                            false
                        );
                        if (result.code === "00") {
                            if (document.querySelector(".mypage_modal.show")) {
                                document
                                    .querySelector(".mypage_modal.show")
                                    .classList.remove("show");
                            }
                            document.querySelector("body").classList.remove("scroll_lock");

                            // 인증번호 적용
                            $("#join-mobile-seq").val(paramValidation["auth_seq"]);
                            $("#mobile_auth").hide();
                            $("#mobile").html($("#usernumber").val());

                            $.mypageUtils.mobileValidationState = "";
                            $.mypageUtils.authSeq = "";

                            // 버튼 상태 초기화
                            document.querySelector("#usernumber").removeAttribute("disabled");
                            document.querySelector("#usernumber").value = '';
                            document.querySelector("#usercode").setAttribute("disabled", "disabled");
                            document.querySelector("#usercode").value = '';
                            document.querySelector("#member_input_message").innerHTML = '';
                            $("#btn-validation-request").show();
                            $("#btn-validation-cancel").hide();
                            $("#btn-validation-confirm").addClass("disabled");
                            $("#btn-validation-re-request").removeClass("disabled").hide();
                        } else if (result.code === "90") {
                            $("#member_input_message").html(ka.msg.join.reqCodeNotMatch);
                        } else if (result.code.indexOf("ka.")) {
                            $("#member_input_message").html(eval(result.code));
                        }
                    }
                } else if (mode === "C") {
                    $.mypageUtils.mobileValidationState = "";
                    $.mypageUtils.authSeq = "";

                    // 버튼 처리
                    $("#btn-validation-cancel").hide();
                    $("#btn-validation-request").show();
                    $("#btn-validation-confirm").attr("disabled", "disabled");
                    $("#btn-validation-confirm").addClass("disabled");

                    // 입력필드 처리
                    $("#usernumber").removeAttr("disabled");
                    $("#usernumber").val("");
                    $("#usercode").attr("disabled", "disabled");
                    $("#usercode").val("");
                } else if (mode === "X") {
                }
            },

            handleFile: function (obj, target) {
                if (obj.length > 0) {
                    var pattern = /[\{\}\/?,;:|*~`!^\+<>@\#$%&\\\=\'\"]/gi;
                    if (pattern.test(obj[0].name)) {
                        $.commonUtils.alert(ka.msg.fileNameCheck);
                    } else {
                        if (
                            $.commonUtils.getFileExt(obj[0].name).toLowerCase() !== "heic" &&
                            $.commonUtils.getFileExt(obj[0].name).toLowerCase() !== "jpg" &&
                            $.commonUtils.getFileExt(obj[0].name).toLowerCase() !== "jpeg" &&
                            $.commonUtils.getFileExt(obj[0].name).toLowerCase() !== "png" &&
                            $.commonUtils.getFileExt(obj[0].name).toLowerCase() !== "pdf"
                        ) {
                            $.commonUtils.alert(ka.msg.join.fileExtension);
                        } else {
                            $("#" + target).html(obj[0].name);
                        }
                    }
                }
            },

            checkBusinessNum: function (target) {
                let d = $("#" + target)
                    .val()
                    .replace(/-/gi, "");
                if (d.replace(/ /gi, "") === "") {
                    $("#pop-business-num-result").html(ka.msg.mypage.companyRegNoEmpty);
                    return false;
                }
                if ($.validateUtils.checkBusinessNum(d)) {
                    $("#company_reg_no").html(d);
                    $("#join-company-reg-no").val(d);

                    try {
                        document
                            .querySelector(".mypage_modal.show")
                            .classList.remove("show");
                        document.querySelector("body").classList.remove("scroll_lock");
                    } catch (e) { }
                } else {
                    $("#pop-business-num-result").html(
                        "국세청 데이터에 존재하지 않는 번호입니다."
                    );
                }
            },
        }),
        ($.cardUtils = {
            emptyResultSrc: "/img/icons/notresult-ico@1x.png",
            emptyResultSrcset:
                "/img/icons/notresult-ico@1x.png 1x, /img/icons/notresult-ico@2x.png 2x, /img/icons/notresult-ico@3x.png 3x",
            emptyBidSrc: "/img/icons/notbid-ico@1x.png",
            emptyBidSrcset:
                "/img/icons/notbid-ico@1x.png 1x, /img/icons/notbid-ico@2x.png 2x, /img/icons/notbid-ico@3x.png 3x",

            /*------------------------------------------------------------------
              * @function:    setEmptyCardTag
              * @param:       target (태그를 붙일 상위 노드)
              * @description: 경매 작품 리스트 (검색 결과가 없을 경우) Card 형식의 object 생성하여 target 에 추가
              ------------------------------------------------------------------*/
            setEmptyCardTag: function (target, mode) {
                var util = $.cardUtils;
                var container = $("<div />", {
                    class: "container m-t-50 m-b-30 p-b-10",
                });
                var icoPage = $("<div />", { class: "ico-page text-center" });

                let msg;
                if (mode === "search" || mode === "schedule") {
                    switch (mode) {
                        case "search":
                            msg = ka.msg.list.emptySeach;
                            break;
                        case "schedule":
                            msg = ka.msg.list.emptySchedule;
                            break;
                        case "wishlist":
                            msg = ka.msg.list.emptyWishlist;
                            break;
                    }
                    icoPage.append(
                        $("<img />", {
                            src: util.emptyResultSrc,
                            alt: msg,
                            srcset: util.emptyResultSrcset,
                        }),
                        $("<p />").append(msg)
                    );
                } else if (
                    mode === "bid" ||
                    mode === "successfulBid" ||
                    mode === "wishlist"
                ) {
                    switch (mode) {
                        case "bid":
                            msg = ka.msg.list.emptyBid;
                            break;
                        case "successfulBid":
                            msg = ka.msg.list.emptySuccessfulBid;
                            break;
                        case "wishlist":
                            msg = ka.msg.list.emptyWishlist;
                            break;
                    }
                    icoPage.append(
                        $("<img />", {
                            src: util.emptyBidSrc,
                            alt: msg,
                            srcset: util.emptyBidSrcset,
                        }),
                        $("<p />").append(msg)
                    );
                }
                $(target).append(container.append(icoPage));
            },

            /*------------------------------------------------------------------
              * @function:    setWorkCardTag
              * @param:       target (태그를 붙일 상위 노드)
              *               item (AuctionWork Object)
              * @description: 경매 작품 리스트 Card 형식의 object 생성하여 target 에 추가
              ------------------------------------------------------------------*/
            setWorkCardTag: function (target, item, auction, index, login) {
                var existBidYN = parseInt(item.bid_cnt) > 0;
                var container = $("<div />", { class: "col mb-4 list-pd" });

                // Card
                var card = $("<div />", {
                    class: "card artwork card-" + (index + 1).toString() + "",
                    style: "height: 500px;",
                });
                // 2022.05.05 :: 홈페이지 기록 삭제 시 D 코드 처리 (조건 추가)
                if (
                    item.hide_yn === "Y" ||
                    item.exhi_yn === "N" ||
                    item.exhi_yn === "U" ||
                    item.exhi_yn === "D"
                ) {
                    if (
                        item.hide_yn !== "Y" &&
                        (item.exhi_yn === "N" ||
                            item.exhi_yn === "U" ||
                            item.exhi_yn === "D")
                    ) {
                        card.append(
                            $("<div />", {
                                style:
                                    "width: 100%; height: 480px; display: flex; align-items: center; justify-content: center; flex-direction: column;",
                            }).append(
                                $("<img />", {
                                    style: "max-height: initial; height: auto; padding: 0;",
                                    src: "/img/icons/work-calcel.png",
                                }),
                                item.exhi_yn === "D"
                                    ? null
                                    : $("<span />", {
                                        style:
                                            "font-size: 14px; color: #dcdcdc; font-family: 'Montserrat', sans-serif; font-weight: 500;",
                                    }).append("Lot " + item.lot_num),
                                item.exhi_yn === "D"
                                    ? null
                                    : $("<p />", {
                                        style:
                                            "font-size: 14px; color: #dcdcdc; text-align: center;",
                                    }).append(ka.msg.list.cancelExhi)
                            )
                        );
                        container.addClass("card-empty");
                    } else {
                        if (index % 9 === 0) {
                            card.append(
                                $("<div />", {
                                    class: "card-next-step",
                                    style: "color: #F47104; margin: 200px 20px 20px 20px;",
                                }).append(ka.msg.auction.nextstep)
                            );
                        } else {
                            card.append(
                                $("<div />", {
                                    style:
                                        "width: 100%; height: 480px; display: flex; align-items: center; justify-content: center;",
                                }).append(
                                    $("<img />", {
                                        src: "/img/kauctionlog@1x.png",
                                        style: "max-width: 100%;",
                                    })
                                )
                            );
                            container.addClass("card-empty");
                        }
                    }
                } else {
                    // Card > Image
                    var image;

                    var closedInfo;

                    if (
                        !(
                            (item.auc_kind === "1" &&
                                (item.active_yn === "Y" || item.preview_yn === "Y")) ||
                            ((item.auc_kind === "2" || item.auc_kind === "4") &&
                                (item.bid_yn === "Y" || item.preview_yn === "Y"))
                        )
                    ) {
                        closedInfo = $("<div />", { class: "Deadline-lot" }).append(
                            $("<span />").append(ka.msg.list.closedLot)
                        );
                    }

                    if (
                        item.minimal_image_yn === "N" &&
                        item.work_link !== "" &&
                        ((item.auc_kind === "1" &&
                            (item.active_yn === "Y" || item.preview_yn === "Y")) ||
                            ((item.auc_kind === "2" || item.auc_kind === "4") &&
                                (item.bid_yn === "Y" || item.preview_yn === "Y")))
                    ) {
                        image = $("<a />", {
                            class: "listimg img-lazy",
                            href:
                                item.work_link + $.stringUtils.jsonToQueryString(requestParam),
                            title:
                                "Lot." +
                                item.lot_num +
                                " - " +
                                item.artist_name +
                                " - " +
                                item.title,
                        })
                            .append(closedInfo)
                            .append(
                                $("<img />", {
                                    loading: "lazy",
                                    style: "cursor: pointer;",
                                    class: "card-img-top",
                                    onError: "this.src='/img/list_noimg.jpg'",
                                    "data-src":
                                        item.thum_file_name === null
                                            ? "/img/list_noimg.jpg"
                                            : $.cardUtils.getImgFolder(
                                                item.auc_kind,
                                                item.auc_num,
                                                item.auc_kind === "1" ? "/T" : ""
                                            ) +
                                            "/" +
                                            item.thum_file_name.replace(".jpg", "_L.jpg"),
                                })
                            );
                    } else {
                        image = $("<a />", {
                            class: "listimg img-lazy",
                            href:
                                item.work_link + $.stringUtils.jsonToQueryString(requestParam),
                            title:
                                "Lot." +
                                item.lot_num +
                                " - " +
                                item.artist_name +
                                " - " +
                                item.title,
                        })
                            .append(closedInfo)
                            .append(
                                $("<img />", {
                                    loading: "lazy",
                                    class: "card-img-top",
                                    onError: "this.src='/img/list_noimg.jpg'",
                                    "data-src":
                                        item.thum_file_name === null
                                            ? "/img/list_noimg.jpg"
                                            : $.cardUtils.getImgFolder(
                                                item.auc_kind,
                                                item.auc_num,
                                                item.auc_kind === "1" ? "/T" : ""
                                            ) +
                                            "/" +
                                            item.thum_file_name,
                                })
                            );

                        if (item.minimal_image_yn === "Y") {
                            image.on("contextmenu", function (e) {
                                return false;
                            });
                        }
                    }

                    // Card > Body
                    var body = $("<div />", { class: "card-body" });

                    // Card > Body > Lot
                    var lot = $("<div />", { class: "lot" }).append(
                        "Lot " + item.lot_num
                    );

                    // Card > Body > Wishlist
                    var wishlist = $("<div />", { class: "wishlist" }).append(
                        $("<div />", {
                            class: "heartic ic",
                            style: "cursor: pointer;",
                            onclick:
                                "$.wishlist.add(" +
                                item.uid +
                                ", '" +
                                (item.wish_yn === "Y" ? "N" : "Y") +
                                "');",
                        }).append(
                            $("<i />", {
                                class: "fa-heart" + (item.wish_yn === "Y" ? " fas" : " far"),
                                style: "cursor: pointer;",
                                onclick:
                                    "$.wishlist.add(" +
                                    item.uid +
                                    ", '" +
                                    (item.wish_yn === "Y" ? "N" : "Y") +
                                    "')",
                            })
                        )
                    );

                    // Card > Body > Title
                    var title = $("<h5 />", { class: "card-title text-truncate" }).append(
                        item.artist_name
                    );

                    var subTitle = $("<div />", { class: "card-text-subtitle" }).append(
                        $("<h5 />", {
                            class: "card-subtitle text-truncate",
                            title: item.title,
                        }).append(item.title),
                        $("<p />", { class: "description text-truncate" }).append(
                            $("<span />", {
                                title: item.material + " " + item.display_edition,
                            }).append(item.material + item.display_edition),
                            $("<br />"),
                            $("<span />", {
                                class: "text-truncate",
                                title:
                                    item.size +
                                    (item.make_date === "" ? "" : " | " + item.make_date),
                            }).append(
                                item.size +
                                (item.make_date === "" ? "" : " | " + item.make_date)
                            )
                        )
                    );

                    var currency = item.hongkong_yn === "Y" ? "HKD" : "KRW";

                    // Card > Body > MainText
                    var mainText = $("<div />", { class: "card-text dotted" }).append(
                        $("<ul />", { class: "list-inline" }).append(
                            $("<li />", { class: "list-inline-item" }).append(
                                item.disp_type !== "R" ||
                                    item.admin_yn === "Y" ||
                                    item.active_yn === "Y"
                                    ? ka.msg.auction.estimate
                                    : ""
                            ),
                            $("<li />", {
                                class: "list-inline-item font-numbers pull-right text-right",
                            }).append(
                                item.disp_type !== "R" ||
                                    item.admin_yn === "Y" ||
                                    item.active_yn === "Y"
                                    ? item.separate_inquiry_yn === "Y"
                                        ? ka.msg.auction.separateInquiryYN
                                        : currency +
                                        " " +
                                        $.stringUtils.comma(item.price_estimated_low) +
                                        "<br />~ " +
                                        $.stringUtils.comma(item.price_estimated_high)
                                    : ""
                            )
                        ),
                        $("<div />", { class: "clearfix" })
                    );

                    if (item.auc_kind !== "1") {
                        mainText.append(
                            $("<ul />", { class: "list-inline" }).append(
                                $("<li />", { class: "list-inline-item" }).append(
                                    ka.msg.auction.starting
                                ),
                                $("<li />", {
                                    class: "list-inline-item font-numbers pull-right text-right",
                                }).append(
                                    currency + " " + $.stringUtils.comma(item.price_start)
                                )
                            )
                        );

                        if (item.bid_yn === "Y" || item.preview_yn === "Y") {
                            mainText.append(
                                $("<ul />", { class: "list-inline" }).append(
                                    $("<li />", {
                                        class: "list-inline-item",
                                        style: "font-weight: bold; color: #ff5d16",
                                    }).append(item.bid_cnt > 0 ? ka.msg.main.current : ""),
                                    item.bid_cnt > 0
                                        ? $("<li />", {
                                            class:
                                                "list-inline-item font-numbers text-primary pull-right",
                                            style: "font-weight: bold;",
                                        }).append(
                                            currency + " " + $.stringUtils.comma(item.price_max)
                                        )
                                        : $("<li />", {
                                            class:
                                                "list-inline-item font-numbers text-primary pull-right",
                                            style: "font-weight: bold;",
                                        }).append("") // KRW - 제거
                                )
                            );
                        } else {
                            if (item.active_yn === "Y") {
                                mainText.append(
                                    $("<ul />", { class: "list-inline" }).append(
                                        $("<li />", {
                                            class: "list-inline-item",
                                            style: "font-weight: bold;",
                                        }).append(ka.msg.auction.hammer),
                                        item.bid_cnt > 0
                                            ? $("<li />", {
                                                class:
                                                    "list-inline-item font-numbers text-primary pull-right",
                                                style: "font-weight: bold;",
                                            }).append(
                                                currency + " " + $.stringUtils.comma(item.price_max)
                                            )
                                            : $("<li />", {
                                                class:
                                                    "list-inline-item font-numbers text-primary pull-right",
                                            }).append(ka.msg.auction.failure)
                                    )
                                );
                            } else {
                                if (isLogin === "True") {
                                    if (item.price_hammer > 0) {
                                        mainText.append(
                                            $("<ul />", { class: "list-inline" }).append(
                                                $("<li />", {
                                                    class: "list-inline-item text-primary",
                                                    style: "font-weight: bold;",
                                                }).append(ka.msg.auction.hammer),
                                                $("<li />", {
                                                    class:
                                                        "list-inline-item font-numbers pull-right text-right text-primary fw-800",
                                                }).append(
                                                    item.disp_type !== "R" || item.admin_yn === "Y"
                                                        ? $.stringUtils.comma(
                                                            currency + " " + item.price_hammer
                                                        )
                                                        : ""
                                                )
                                            )
                                        );
                                    } else {
                                        mainText.append(
                                            $("<ul />", { class: "list-inline" }).append(
                                                $("<li />", { class: "list-inline-item" }),
                                                $("<li />", {
                                                    class:
                                                        "list-inline-item font-numbers pull-right text-right text-primary fw-800",
                                                }).append(ka.msg.auction.failure)
                                            )
                                        );
                                    }
                                }
                            }
                        }
                    } else {
                        if (isLogin === "True" && item.live_yn !== "Y") {
                            if (item.price_hammer > 0) {
                                mainText.append(
                                    $("<ul />", { class: "list-inline" }).append(
                                        $("<li />", {
                                            class: "list-inline-item text-primary",
                                            style: "font-weight: bold;",
                                        }).append(ka.msg.auction.hammer),
                                        $("<li />", {
                                            class:
                                                "list-inline-item font-numbers pull-right text-right text-primary fw-800",
                                        }).append(
                                            item.disp_type !== "R" || item.admin_yn === "Y"
                                                ? $.stringUtils.comma(
                                                    currency + " " + item.price_hammer
                                                )
                                                : ""
                                        )
                                    )
                                );
                            } else {
                                //mainText.append($("<ul />", { "class": "list-inline" }).append(
                                //    $("<li />", { "class": "list-inline-item" }),
                                //    $("<li />", { "class": "list-inline-item font-numbers pull-right text-right text-primary fw-800" }).append(ka.msg.auction.failure)
                                //));
                            }
                        }
                    }

                    // Card > Body > SubText
                    var subText = $("<div />", { class: "card-text" }).append(
                        $("<ul />", { class: "list-inline" }).append(
                            $("<li />", { class: "list-inline-item" }).append(
                                $("<span />", {
                                    style: item.auc_kind === "1" ? "color: #F47104" : "",
                                }).append(item.card_message)
                            ),
                            item.auc_kind !== "1" && item.bid_cnt > 0
                                ? $("<li />", { class: "list-inline-item pull-right" }).append(
                                    $("<span />", {
                                        onclick: "$.mypageUtils.openBidList(" + item.uid + ");",
                                        class: "font-numbers text-primary",
                                        style: "font-weight: bold; cursor: pointer;",
                                    }).append(
                                        item.bid_cnt.toString() +
                                        " " +
                                        (item.bid_cnt === 1
                                            ? ka.msg.auction.bidCount2.replace("Bids", "Bid")
                                            : ka.msg.auction.bidCount2)
                                    )
                                )
                                : ""
                        )
                    );

                    var links = [lot, title, subTitle, mainText];
                    $.each(links, function (i, obj) {
                        if (
                            item.work_link !== "" &&
                            (item.bid_yn === "Y" || item.preview_yn === "Y")
                        ) {
                            obj.css("cursor", "pointer");
                            obj.attr(
                                "onclick",
                                "window.open('" + item.work_link + "', '_self');"
                            );
                        }
                    });

                    if (item.auc_kind === "1" && item.live_yn === "Y") {
                        subText.append(
                            $("<div />").append(
                                $("<button />", {
                                    class: "btn btn-block btn-md m-t-10 m-b-10",
                                    onclick:
                                        item.separate_inquiry_yn === "Y"
                                            ? "$.commonUtils.alert('" +
                                            ka.msg.auction.separateInquiry +
                                            "')"
                                            : "$.bid.redirectMajorBid(" + item.uid + ", event);",
                                }).append(ka.msg.auction.placebid)
                            )
                        );
                    }

                    var copyright;
                    if (item.copyright !== "") {
                        copyright = $("<div />", { class: "text-copyright" }).append(
                            $("<p />", { class: "text-truncate" }).append(
                                $("<span />", { class: "fs-10", title: item.copyright }).append(
                                    item.copyright
                                )
                            )
                        );
                    }

                    card.append(
                        image,
                        body.append(
                            lot,
                            wishlist,
                            title,
                            subTitle,
                            mainText,
                            subText,
                            copyright
                        )
                    );
                }

                if (item.badge && item.badge !== "") {
                    let charlity = $("<div />", { class: "charlity" }).append(
                        $("<span />").append(item.badge)
                    );
                    charlity.hover(
                        function () {
                            $(this).find("> span").text(item.badge_desc);
                        },
                        function () {
                            $(this).find("> span").text(item.badge);
                        }
                    );
                    card.append(charlity);
                }

                $(target).append(container.append(card));
            },

            /*------------------------------------------------------------------
              * @function:    setWorkHorizonalCard
              * @param:       target (태그를 붙일 상위 노드)
              *               item (AuctionWork Object)
              *               mode (특정 요소를 표현할 모드)
              * @description: 경매 작품 리스트 Card 형식의 object 생성하여 target 에 추가 (가로형)
              ------------------------------------------------------------------*/
            setWorkHorizonalCard: function (target, item, index, mode) {
                if (item.hide_yn === "Y") return;

                // Card
                const container = $("<div />", { class: "card-list m-b-10" });
                const row = $("<div />", { class: "row-pad" });
                let imageInfo, workInfo, aucInfo;

                if (
                    item.hide_yn === "Y" ||
                    item.exhi_yn === "N" ||
                    item.exhi_yn === "U"
                ) {
                    if (
                        item.hide_yn !== "Y" &&
                        (item.exhi_yn === "N" || item.exhi_yn === "U")
                    ) {
                        // Card > Image
                        imageInfo = $("<div />", { class: "card-list-img" }).append(
                            $("<img />", {
                                loading: "lazy",
                                src: "/img/icons/work-calcel-dark.png",
                            })
                        );

                        // Card > WorkInfo
                        workInfo = $("<div />", {
                            class: "card-list-info work-cancel",
                            style: "position: relative; height: 100%;",
                        }).append(
                            $("<div />", {
                                class: "card-lot-num",
                                style:
                                    "text-transform: uppercase;font-size: 14px; color: #ccc;",
                            }).append("LOT. " + item.lot_num),
                            $("<p />", { style: "font-size: 14px; color: #ccc;" }).append(
                                ka.msg.list.cancelExhi
                            )
                        );

                        container.append(
                            row.append(
                                imageInfo,
                                workInfo,
                                $("<div />", { class: "card-price-info" })
                            )
                        );
                    }
                } else {
                    // Card > Image
                    let fileName = "/img/list_noimg.jpg";
                    if (
                        item.minimal_image_yn === "N" &&
                        (item.bid_yn === "Y" || item.active_yn === "Y")
                    ) {
                        if (typeof item.thum_file_name === "string") {
                            fileName =
                                $.cardUtils.getImgFolder(
                                    item.auc_kind,
                                    item.auc_num,
                                    item.auc_kind === "1" ? "/T" : ""
                                ) +
                                "/" +
                                item.thum_file_name.replace(".jpg", "_L.jpg");
                        }
                    } else {
                        if (typeof item.img_file_name === "string") {
                            fileName =
                                $.cardUtils.getImgFolder(
                                    item.auc_kind,
                                    item.auc_num,
                                    item.auc_kind === "1" ? "/T" : ""
                                ) +
                                "/" +
                                item.thum_file_name;
                        }
                    }

                    if (item.work_link !== "") {
                        imageInfo = $("<div />", {
                            class: "card-list-img",
                            style: "cursor: pointer;",
                        })
                            .append(
                                $("<img />", {
                                    loading: "lazy",
                                    src: fileName,
                                    onError: "this.src='/img/list_noimg.jpg'",
                                })
                            )
                            .append(
                                $("<div />", {
                                    class: "heartic", // ,
                                    // "onclick": "$.wishlist.add(" + item.uid + ", '" + (item.wish_yn === "Y" ? "N" : "Y") + "');"
                                }).append(
                                    $("<i />", {
                                        class:
                                            item.wish_yn === "Y" ? "fas fa-heart" : "far fa-heart",
                                        "data-wish": item.wish_yn,
                                        "data-uid": item.uid,
                                        id: "icon-w-" + item.uid,
                                        onclick:
                                            "$.wishlist.add(" +
                                            item.uid +
                                            ", '" +
                                            (item.wish_yn === "Y" ? "N" : "Y") +
                                            "')",
                                    })
                                )
                            );
                    } else {
                        imageInfo = $("<div />", { class: "card-list-img" })
                            .append(
                                $("<img />", {
                                    loading: "lazy",
                                    src: fileName,
                                    onError: "this.src='/img/list_noimg.jpg'",
                                })
                            )
                            .append(
                                $("<div />", {
                                    class: "heartic", // ,
                                    // "onclick": "$.wishlist.add(" + item.uid + ", '" + (item.wish_yn === "Y" ? "N" : "Y") + "')"
                                }).append(
                                    $("<i />", {
                                        class:
                                            item.wish_yn === "Y" ? "fas fa-heart" : "far fa-heart",
                                        "data-wish": item.wish_yn,
                                        "data-uid": item.uid,
                                        id: "icon-w-" + item.uid,
                                        onclick:
                                            "$.wishlist.add(" +
                                            item.uid +
                                            ", '" +
                                            (item.wish_yn === "Y" ? "N" : "Y") +
                                            "')",
                                    })
                                )
                            );
                    }

                    if (item.minimal_image_yn === "Y") {
                        imageInfo.on("contextmenu", function (e) {
                            return false;
                        });
                    }

                    // Card > WorkInfo
                    workInfo = $("<div />", { class: "card-list-info" });

                    if (item.work_link !== "") {
                        workInfo.attr("style", "cursor: pointer;");
                        workInfo.attr(
                            "onclick",
                            "window.open('" + item.work_link + "', '_self');"
                        );
                    }

                    // Card > WorkInfo > AucTitle (Mode: Search)
                    if (mode === "search") {
                        workInfo.append(
                            $("<h5 />", { class: "card-sub-date-auction" }).append(
                                item.auc_title
                            )
                        );
                    }

                    // Card > WorkInfo > Lot
                    workInfo.append(
                        $("<div />", { class: "card-lot-num" }).append(
                            "LOT. " + item.lot_num
                        )
                    );

                    // Card > WorkInfo > Artist
                    const artist = $.commonUtils.isNull(item.direct_date)
                        ? item.artist_name
                        : item.artist_name +
                        "<font size='2'>&nbsp;(" +
                        item.direct_date +
                        ")</font>";
                    workInfo.append($("<h5 />", { class: "card-writer" }).append(artist));

                    // Card > WorkInfo > Title
                    workInfo.append(
                        $("<h5 />", { class: "card-subtitle-desc" }).append(item.title)
                    );

                    // Card > WorkInfo > Etc
                    let etc = item.material + item.display_edition;
                    etc += "<br/>";
                    etc +=
                        item.size + (item.make_date === "" ? "" : " | " + item.make_date);

                    workInfo.append(
                        $("<h5 />", { class: "card-subtitle-infodesc" }).append(etc)
                    );

                    // Card > AucInfo
                    aucInfo = $("<div />", { class: "card-price-info" });
                    const aucList = $("<div />", { class: "card-price-list" });
                    const aucButton = $("<div />", { class: "card-price-btn" }); // wishList 가 없을 경우 open 추가
                    const existBidYN = parseInt(item.bid_cnt) > 0;

                    if (item.auc_kind === "1") {
                        aucList.append($.cardUtils.getDataTag("estimate", item));
                        if (item.active_yn === "N") {
                            aucList.append($.cardUtils.getDataTag("hammer", item, "Y"));
                        }
                    } else {
                        if (item.bid_yn == "Y") {
                            if (item.is_login) {
                                if (existBidYN) {
                                    aucList.append($.cardUtils.getDataTag("current", item, "Y"));
                                }
                            }
                        } else {
                            if (item.active_yn === "Y") {
                                if (existBidYN) {
                                    aucList.append(
                                        $.cardUtils.getDataTag("current", item, "Y", true)
                                    );
                                }
                            } else {
                                aucList.append($.cardUtils.getDataTag("hammer", item, "Y"));
                            }
                            if (item.preview_yn !== "Y") {
                                imageInfo.append(
                                    $("<div />", { class: "bid-closed-desc" }).append(
                                        ka.msg.list.closedLot
                                    )
                                );
                            }
                        }

                        aucList.append(
                            $.cardUtils.getDataTag("starting", item),
                            $.cardUtils.getDataTag("estimate", item),
                            $.cardUtils.getDataTag("endTimeHorizonal", item)
                        );

                        if ((item.active_yn === "Y" || item.is_login) && existBidYN) {
                            aucList.append($.cardUtils.getDataTag("bidCount", item));
                        }
                    }

                    if (mode === "wishlist") {
                        aucList.append($.cardUtils.getDataTag("wishRegDate", item));
                    }

                    if (item.copyright !== "") {
                        aucList.append(
                            $("<ul />", { class: "list-inline copy" }).append(
                                $("<li />", { class: "list-inline-item" }).append(
                                    item.copyright
                                )
                            )
                        );
                    }

                    if (item.auc_kind === "1") {
                        if (item.active_yn === "Y" && item.live_yn === "Y") {
                            aucButton.append(
                                $("<a />", {
                                    onclick: "$.bid.redirectMajorBid(" + item.uid + ", event);",
                                }).append("<span>" + ka.msg.auction.placebid + "</span>")
                            );
                        }
                    } else {
                        if (item.bid_yn === "Y") {
                            aucButton.append(
                                $("<a />", {
                                    onclick:
                                        "window.location.href='" +
                                        $.cardUtils.getGoToBidLink(item) +
                                        "';",
                                }).append("<span>" + ka.msg.list.goBid + "</span>")
                            );
                        } else {
                            if (mode === "wishlist" || mode === "list") {
                                if (item.preview_yn !== "Y") {
                                    aucButton.append(
                                        $("<a />", {
                                            onclick: "$.mypageUtils.openBidList(" + item.uid + ");",
                                        }).append("<span>" + ka.msg.list.viewResult + "</span>")
                                    );
                                }
                            }
                        }
                    }

                    aucInfo.append(aucList, aucButton);

                    /* 10월5일수정 */
                    container.append(row.append(imageInfo, workInfo, aucInfo));
                }
                $(target).append(container);
            },

            /*------------------------------------------------------------------
              * @function:    setScheduleCardTag
              * @param:       target (태그를 붙일 상위 노드)
              *               item (AuctionSchedule Object)
              * @description: 경매 결과 리스트 Card 형식의 object 생성하여 target 에 추가
              ------------------------------------------------------------------*/
            setScheduleCardTag: function (target, item, auction) {
                var container = $("<div />", { class: "item col mb-4" });

                var card = $("<div />", {
                    class: "card auction ca",
                    style: "cursor: pointer;",
                });
                card.on("click", function () {
                    if (item.auth_yn === "Y") {
                        window.open(
                            "/Auction/" +
                            $.cardUtils.getAuctionTitle(item.auc_kind) +
                            "/" +
                            item.auc_num.toString(),
                            "_self"
                        );
                    } else {
                        if (item.auc_kind === "1") {
                            $.commonUtils.alert(ka.msg.list.denyScheduleLive);
                        } else {
                            $.commonUtils.alert(ka.msg.list.denyScheduleOnline);
                        }
                    }
                });

                // Card > Image
                var image = $("<div />", { class: "auction-image img-lazy" });
                if (item.img_file_name !== null) {
                    image.append(
                        $("<img />", {
                            onError: "this.src='/img/list_noimg.jpg'",
                            "data-src":
                                $.cardUtils.getListImgFolder(item.auc_kind, item.auc_num) +
                                "/" +
                                item.img_file_name.replace(".jpg", "_L.jpg"),
                            class: "card-img-top",
                        })
                    );
                }

                // Card > Content
                var content = $("<div />", { class: "auction-content" });

                // Card > Content > Body
                var body = $("<div />", { class: "card-body" });

                // Card > Content > Body > Title
                var title = $("<div />").append(
                    $("<h5 />", { class: "card-subtitle" }).append(item.auc_title)
                );

                // Card > Content > Body > Text
                var text = $("<div />", { class: "card-text" }).append(
                    title //,
                    //$("<ul />", { "class": "list-inline" }).append(
                    //    $("<li />", { "class": "list-inline-item" }).append(ka.msg.auction.place),
                    //    $("<li />", { "class": "list-inline-item pull-right" }).append(item.auc_place)
                    //),
                    //$("<ul />", { "class": "list-inline" }).append(
                    //    $("<li />", { "class": "list-inline-item" }).append(item.auc_kind === "1" ? ka.msg.auction.date : ka.msg.auction.startDate),
                    //    $("<li />", { "class": "list-inline-item pull-right" }).append((item.auc_kind === "1") ? item.auc_date : item.auc_start_date)
                    //)
                );

                //var textContent = $('<div />', { "class": "listAll" }).append(
                //    (item.auc_kind !== "1" ? $("<ul />", { "class": "list-inline" }).append(
                //        $("<li />", { "class": "list-inline-item" }).append(ka.msg.auction.place),
                //        $("<li />", { "class": "list-inline-item pull-right" }).append(item.auc_place)
                //    ) : null),
                //    $("<ul />", { "class": "list-inline" }).append(
                //        $("<li />", { "class": "list-inline-item" }).append(item.auc_kind === "1" ? ka.msg.auction.date : ka.msg.auction.startDate),
                //        $("<li />", { "class": "list-inline-item pull-right" }).append((item.auc_kind === "1") ? item.auc_date : item.auc_start_date)
                //    )
                //    , (item.auth_yn === "Y" ? ($('<div />', { "class": "list-more-btn" }).append(
                //        $('<button />').append(
                //            "상세보기",
                //            $('<i />', { "class": "fas fa-arrow-right" })
                //    ))) : null)
                //);
                var textContent = $("<div />", { class: "listAll" });
                if (item.auc_kind === "1") {
                    textContent.append(
                        $("<ul />", { class: "list-inline" }).append(
                            $("<li />", { class: "list-inline-item" }).append(
                                ka.msg.auction.place
                            ),
                            $("<li />", { class: "list-inline-item pull-right" }).append(
                                item.auc_place
                            )
                        )
                    );
                }

                textContent.append(
                    $("<ul />", { class: "list-inline" }).append(
                        $("<li />", { class: "list-inline-item" }).append(
                            item.auc_kind === "1"
                                ? ka.msg.auction.date
                                : ka.msg.auction.startDate
                        ),
                        $("<li />", { class: "list-inline-item pull-right" }).append(
                            item.auc_kind === "1" ? item.auc_date : item.auc_start_date
                        )
                    )
                );

                if (item.auc_kind !== "1") {
                    //text.append(
                    //    $("<ul />", { "class": "list-inline" }).append(
                    //        $("<li />", { "class": "list-inline-item" }).append(ka.msg.auction.endDate),
                    //        $("<li />", { "class": "list-inline-item pull-right" }).append(item.auc_end_date)
                    //    )
                    //);
                    textContent.append(
                        $("<ul />", { class: "list-inline" }).append(
                            $("<li />", { class: "list-inline-item" }).append(
                                ka.msg.auction.endDate
                            ),
                            $("<li />", { class: "list-inline-item pull-right" }).append(
                                item.auc_end_date
                            )
                        )
                    );
                }

                if (item.auth_yn === "Y") {
                    textContent.append(
                        $("<div />", { class: "list-more-btn" }).append(
                            $("<button />").append(
                                ka.msg.list.viewDetails,
                                $("<i />", { class: "fas fa-arrow-right" })
                            )
                        )
                    );
                }

                text.append(textContent);

                content.append(body.append(text));

                card.append(image, content);

                $(target).append(container.append(card));
            },

            /*------------------------------------------------------------------
              * @function:    setWishWorkCardTag
              * @param:       target (태그를 붙일 상위 노드)
              *               item (AuctionWork Object)
              * @description: 경매 결과 리스트 Card 형식의 object 생성하여 target 에 추가
              ------------------------------------------------------------------*/
            setWishWorkCardTag: function (target, item, lang) {
                //var container = $("<div />", { "class": "col" });

                //// Card
                //var cardCont = $("<div />", { "class": "card artwork", "style": "min-height: 200px;" });
                //var card = $("<div />", { "class": "row" });

                //if (!(item.hide_yn === "Y" || item.exhi_yn === "N")) {
                //    // Card > Image
                //    var image;
                //    if (item.work_link !== "") {
                //        image = $("<div />", { "class": "col-lg-4" }).append(
                //            $("<a />", { "href": item.work_link }).append(
                //                $("<img />", { "style": "cursor: pointer;", "class": "card-img-top", "src": $.cardUtils.getImgFolder(item.auc_kind, item.auc_num) + '/' + item.thum_file_name.replace(".jpg", "_L.jpg") })
                //            )
                //        );
                //    } else {
                //        image = $("<div />", { "class": "col-lg-4" }).append(
                //            $("<img />", { "class": "card-img-top", "src": $.cardUtils.getImgFolder(item.auc_kind, item.auc_num) + '/' + item.thum_file_name.replace(".jpg", "_L.jpg") })
                //        );
                //    }

                //    // Card > WorkInfo
                //    var workInfo = $("<div />", { "class": "col-lg-4" });

                //    // Card > WorkInfo > Lot
                //    var lot = $("<div />", { "class": "lot" }).append("Lot " + item.lot_num);

                //    // Card > WorkInfo > Wishlist
                //    var wishlist = $("<div />", { "class": "wishlist" }).append(
                //        $("<div />", { "class": item.wish_yn === "Y" ? "heart-select" : "heart", "style": "cursor: pointer;", "onclick": "$.wishlist.add(" + item.work_seq + ", '" + (item.wish_yn === "Y" ? "N" : "Y") + "')" }).append(
                //            $("<i />", { "class": "fas fa-heart" })
                //        )
                //    );

                //    // Card > WorkInfo > Title
                //    var title = $("<h5 />", { "class": "card-title text-truncate" }).append(item.artist_name);

                //    // Card > WorkInfo > SubTitle
                //    var subTitle = $("<h5 />", { "class": "card-subtitle text-truncate" }).append(item.work_title);

                //    subTitle.append($("<p />", { "class": "description text-truncate" }).append(
                //        $("<span />").append((item.make_date === "" ? item.material : item.make_date + " " + item.material)),
                //        $("<br />"),
                //        $("<span />", { "class": "text-truncate" }).append(item.work_size)
                //    ));

                //    // Card > AucInfo
                //    var aucInfo = $("<div />", { "class": "col-lg-4" });

                //    // Card > AucInfo > MainText
                //    var mainText = $("<div />", { "class": "card-text dotted" }).append(
                //        $("<ul />", { "class": "list-inline" }).append(
                //            $("<li />", { "class": "list-inline-item" }).append(lang["추정가"]),
                //            $("<li />", { "class": "list-inline-item font-numbers pull-right text-right" }).append('KRW ' + $.stringUtils.comma(item.price_estimated_low) + '<br />~ ' + $.stringUtils.comma(item.price_estimated_high))
                //        ),
                //        $("<div />", { "class": "clearfix" })
                //    );

                //    if (item.auc_kind !== "1") {
                //        mainText.append($("<ul />", { "class": "list-inline" }).append(
                //            $("<li />", { "class": "list-inline-item" }).append(lang["시작가"]),
                //            $("<li />", { "class": "list-inline-item font-numbers pull-right text-right" }).append('KRW ' + $.stringUtils.comma(item.price_start))
                //        ),
                //            $("<ul />", { "class": "list-inline" }).append(
                //                $("<li />", { "class": "list-inline-item" }).append(lang["현재가"]),
                //                (item.bid_cnt > 0 ? $("<li />", { "class": "list-inline-item font-numbers text-primary pull-right", "style": "font-weight: bold;" }).append('KRW ' + $.stringUtils.comma(item.price_max))
                //                    : $("<li />", { "class": "list-inline-item font-numbers text-primary pull-right" }).append('KRW ' + $.stringUtils.comma(item.price_start)))
                //            )
                //        );
                //    }

                //    // Card > Body > SubText
                //    var subText = $("<div />", { "class": "card-text" }).append(
                //        $("<ul />", { "class": "list-inline" }).append(
                //            $("<li />", { "class": "list-inline-item" }).append(
                //                $("<a />", { "href": "javascript:$.commonUtils.openLogin();", "style": (item.auc_kind === "1" ? "color: #F47104" : "") }).append(item.card_message)
                //            ),
                //            (item.auc_kind !== "1" && item.bid_cnt > 0 ? $("<li />", { "class": "list-inline-item pull-right" }).append(
                //                $("<span />", { "class": "font-numbers text-primary", "style": "font-weight: bold;" }).append(item.bid_cnt.toString() + lang["회"])) : "")
                //        )
                //    );

                //    cardCont.append(card.append(image, workInfo.append(lot, wishlist, title, subTitle), aucInfo.append(mainText, subText)));
                //}

                //$(target).append(container.append(cardCont));

                var container = $("<div />", { class: "col" }); // $("<div />", { "class": "card artwork col-md-12 col-lg-12", "style": "min-height: 200px; border: 1px solid rgba(0,0,0,.125)" });

                // Card
                var cardCont = $("<div />", {
                    class: "card artwork",
                    style: "min-height: 200px;",
                });
                var card = $("<div />", { class: "row", style: "padding: 10px;" });

                // Card > Image
                var imageInfo = $("<div />", { class: "col-md-4 col-lg-4" }).append(
                    $("<img />", {
                        class: "card-img-top",
                        loading: "lazy",
                        onError: "this.src='/img/list_noimg.jpg'",
                        src:
                            $.cardUtils.getImgFolder(
                                item.auc_kind,
                                item.auc_num,
                                item.auc_kind === "1" ? "/T" : ""
                            ) +
                            "/" +
                            item.img_file_name.replace(".jpg", "_L.jpg"),
                    })
                );

                // Card > WorkInfo
                var artist = $.commonUtils.isNull(item.birth_date)
                    ? item.artist_name
                    : item.artist_name + " (" + item.birth_date + ")";

                let etc = item.material + item.display_edition;
                etc += "<br/>";
                etc +=
                    item.size + (item.make_date === "" ? "" : " | " + item.make_date);

                var workInfo = $("<div />", { class: "col-md-5 col-lg-5" }).append(
                    $("<div />", { class: "lot fs-20" }).append("Lot. " + item.lot_num),
                    $("<h5 />", { class: "card-title text-truncate" }).append(artist),
                    $("<h5 />", { class: "card-subtitle text-truncate fs-20" }).append(
                        item.title
                    ),
                    $("<h5 />", { class: "card-subtitle text-truncate" }).append(etc)
                );

                // Card > AucInfo
                if (item.separate_inquiry_yn === "Y") {
                    estimateTag = ka.msg.auction.separateInquiryYN;
                } else {
                    estimateTag =
                        "KRW " +
                        $.stringUtils.comma(item.price_estimated_low) +
                        " ~ " +
                        $.stringUtils.comma(item.price_estimated_high);
                }

                var aucInfo = $("<div />", {
                    class: "col-md-3 col-lg-3 p-t-10",
                }).append(
                    $("<ul />", { class: "list-inline" }).append(
                        $("<li />", { class: "list-inline-item" }).append("낙찰가"),
                        $("<li />", {
                            class: "list-inline-item pull-right text-right",
                        }).append("KRW " + $.stringUtils.comma(item.price_hammer))
                    ),
                    $("<ul />", { class: "list-inline" }).append(
                        $("<li />", { class: "list-inline-item" }).append("시작가"),
                        $("<li />", {
                            class: "list-inline-item pull-right text-right",
                        }).append("KRW " + $.stringUtils.comma(item.price_start))
                    ),
                    $("<ul />", { class: "list-inline" }).append(
                        $("<li />", { class: "list-inline-item" }).append("추정가"),
                        $("<li />", {
                            class: "list-inline-item pull-right text-right text-primary",
                        }).append(estimateTag)
                    )
                );

                if (item.auc_kind === "1") {
                } else {
                    aucInfo.append(
                        $("<ul />", { class: "list-inline" }).append(
                            $("<li />", { class: "list-inline-item" }).append("응찰수"),
                            $("<li />", {
                                class: "list-inline-item pull-right text-right",
                            }).append(item.bid_cnt)
                        ),
                        $("<ul />", { class: "list-inline" }).append(
                            $("<li />", { class: "list-inline-item" }).append("마감"),
                            $("<li />", {
                                class: "list-inline-item pull-right text-right",
                            }).append(item.auc_end_date)
                        ),
                        $("<a />", {
                            class:
                                "btn btn-outline-black btn-lg btn-block btn-icon-right m-t-25",
                            onclick: "$.mypageUtils.openBidList(" + item.uid + ")",
                        }).append(
                            "<span>결과보기</span><i class='far fa-arrow-right p-r-10'></i>"
                        )
                    );
                }

                //if (item.auc_kind === "1") {
                //    aucInfo.append(
                //        $('<ul />', { "class": "list-inline" }).append(
                //            $('<li />', { "class": "list-inline-item" }).append('응찰가'),
                //            $('<li />', { "class": "list-inline-item pull-right text-right text-primary" }).append(item.major_bid_type_name + " KRW " + $.stringUtils.comma(item.price_bid)),
                //        ),
                //        $('<ul />', { "class": "list-inline" }).append(
                //            $('<li />', { "class": "list-inline-item" }).append('진행상태'),
                //            $('<li />', { "class": "list-inline-item pull-right text-right text-primary" }).append(item.major_bp_state_name)
                //        )
                //    );
                //} else {
                //    if (successfulBid === "N") {
                //        aucInfo.append(
                //            $('<a />', { "class": "btn btn-outline-black btn-lg btn-block btn-icon-right m-t-40", "onclick": "$.mypageUtils.openBidList(" + item.uid + ")" }).append("<span>응찰현황</span><i class='far fa-arrow-right p-r-10'></i>")
                //        );
                //    } else {
                //        aucInfo.append(
                //            $('<ul />', { "class": "list-inline" }).append(
                //                $('<li />', { "class": "list-inline-item" }).append('구매가'),
                //                $('<li />', { "class": "list-inline-item pull-right text-right text-primary" }).append("KRW " + $.stringUtils.comma(item.price_purchase))
                //            )
                //        );
                //    }
                //}

                cardCont.append(card.append(imageInfo, workInfo, aucInfo));

                $(target).append(container.append(cardCont));
            },

            /*------------------------------------------------------------------
              * @function:    setWishWorkCardTag
              * @param:       target (태그를 붙일 상위 노드)
              *               msg (Wish List 공백 처리 메세지)
              * @description: 경매 작품 리스트 (검색 결과가 없을 경우) Card 형식의 object 생성하여 target 에 추가
              ------------------------------------------------------------------*/
            setEmptyWishWorkCardTag: function (target, msg) {
                var container = $("<div />", {
                    class: "col",
                    style: "flex: inherit; width: 100%; max-width: 100%;",
                });

                // Card
                var card = $("<div />", { class: "card" });

                // Card > Body
                var body = $("<div />", {
                    class: "card-body",
                    style: "height: 460px; text-align: center; line-height: 260px;",
                })
                    .append(
                        '<svg id="notico" data-name="notico" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 181.4 135.7" width="180px" height="135px"><defs><style>.notico1{opacity:0.61;isolation:isolate;fill:url(#linear-gradient);}.notico2{opacity:0.5;}.notico3{fill:#999;}</style><linearGradient id="linear-gradient" x1="90.7" y1="106.34" x2="90.7" y2="160.19" gradientTransform="matrix(1, 0, 0, -1, 0, 229.89)" gradientUnits="userSpaceOnUse"><stop offset="0" stop-color="#fff" stop-opacity="0"></stop><stop offset="0.5" stop-color="#fdfdfd" stop-opacity="0.5"></stop><stop offset="0.68" stop-color="#f6f6f6" stop-opacity="0.68"></stop><stop offset="0.8" stop-color="#ebebeb" stop-opacity="0.8"></stop><stop offset="0.9" stop-color="#dadada" stop-opacity="0.9"></stop><stop offset="0.99" stop-color="#c4c4c4" stop-opacity="0.99"></stop><stop offset="1" stop-color="#c2c2c2"></stop></linearGradient></defs>' +
                        '<ellipse class="notico1" cx="90.7" cy="101.6" rx="90.7" ry="34.1" ></ellipse >' +
                        '<g class="notico2 ani1">' +
                        '<path class="notico3" d="M115,11.1l9.1-9.5h-8.2V0h11.8V1.2l-9.1,9.5h9.2v1.6H115.1V11.1Z"></path></g>' +
                        '<g class="notico2 ani2">' +
                        '<path class="notico3" d="M132.3,24.4l13.5-14.2H133.5V7.8H151V9.5L137.5,23.7h13.6v2.4H132.2V24.4Z"></path></g>' +
                        '<path class="notico3" d="M65.1,22.2a1.54,1.54,0,0,0-1.5,1.5V48.5a1.5,1.5,0,0,0,3,0V23.7A1.47,1.47,0,0,0,65.1,22.2Z"></path>' +
                        '<path class="notico3" d="M57.4,25a1.54,1.54,0,0,0-1.5,1.5V45.7a1.5,1.5,0,0,0,3,0V26.5A1.54,1.54,0,0,0,57.4,25Z"></path>' +
                        '<path class="notico3" d="M113.9,22.2a1.54,1.54,0,0,0-1.5,1.5V48.5a1.5,1.5,0,0,0,3,0V23.7A1.47,1.47,0,0,0,113.9,22.2Z"></path>' +
                        '<path class="notico3" d="M121.6,25a1.54,1.54,0,0,0-1.5,1.5V45.7a1.5,1.5,0,1,0,3,0V26.5A1.47,1.47,0,0,0,121.6,25Z"></path>' +
                        '<path class="notico3" d="M105.4,24.6H73.7a1.54,1.54,0,0,0-1.5,1.5V46a1.54,1.54,0,0,0,1.5,1.5H88.5V87.7a1.5,1.5,0,0,0,3,0V47.5h13.9a1.54,1.54,0,0,0,1.5-1.5V26.1A1.54,1.54,0,0,0,105.4,24.6Z"></path></svg >'
                    )
                    .append(
                        $("<h5 />", { class: "card-title" }).append(
                            typeof msg !== "undefined" ? msg : "등록된 작품이 없습니다."
                        )
                    );

                $(target).append(container.append(card.append(body)));
            },
            
            getImageDomain: function () {
                return (imageDomain);
            },

            /*------------------------------------------------------------------
              * @function:    getImgFolder
              * @param:       aucKind (경매유형)
              *               aucNum (경매고유번호)
              * @description: 경매별 이미지 메인 경로 처리
              * @returns:     경로 정보 리턴
              ------------------------------------------------------------------*/
            getImgFolder: function (aucKind, aucNum, path) {
                if (aucKind === "2")
                    return (
                        imageDomain +
                        "/www/KMall/Work/" +
                        $.stringUtils.right("0000" + aucNum, 4)
                    );
                else if (aucKind === "4")
                    return (
                        imageDomain +
                        "/www/Konline/Work/" +
                        $.stringUtils.right("0000" + aucNum, 4)
                    );
                else
                    return (
                        imageDomain +
                        "/www/Work/" +
                        $.stringUtils.right("0000" + aucNum, 4) +
                        (typeof path === "string" ? path : "")
                    );
            },

            getListImgFolder: function (aucKind, aucNum, path) {
                if (aucKind === "2")
                    return (
                        imageDomain +
                        "/www/KMall/Work/" +
                        $.stringUtils.right("0000" + aucNum, 4)
                    );
                else if (aucKind === "4")
                    return (
                        imageDomain +
                        "/www/Konline/Work/" +
                        $.stringUtils.right("0000" + aucNum, 4)
                    );
                else
                    return (
                        imageDomain +
                        "/www/Work/" +
                        $.stringUtils.right("0000" + aucNum, 4) +
                        (typeof path === "string" ? path : "") +
                        "/T"
                    );
            },

            /*------------------------------------------------------------------
              * @function:    setBidWorkCardTag
              * @param:       target (태그를 붙일 상위 노드)
              *               item (AuctionWork Object)
              * @description: 응찰 작품 리스트 Card 형식의 object 생성하여 target 에 추가
              ------------------------------------------------------------------*/
            setBidWorkCardTag: function (target, item, successfulBid) {
                var container = $("<div />", {
                    class: "card-list",
                    style: "margin-bottom: 15px;"
                });

                // Card
                var card = $("<div />", { class: "row-pad" });

                var imageInfo = $("<div />", { class: "card-list-img" });
                if (item.work_link !== "") {
                    imageInfo.append(
                        $("<a />", {
                            href: item.work_link,
                            title:
                                "Lot." +
                                item.lot_num +
                                " - " +
                                item.artist_name +
                                " - " +
                                item.title,
                        }).append(
                            $("<img />", {
                                class: "",
                                loading: "lazy",
                                onError: "this.src='/img/list_noimg.jpg'",
                                src:
                                    item.img_file_name === null
                                        ? "/img/list_noimg.jpg"
                                        : $.cardUtils.getImgFolder(
                                            item.auc_kind,
                                            item.auc_num,
                                            item.auc_kind === "1" ? "/T" : ""
                                        ) +
                                        "/" +
                                        item.img_file_name.replace(".jpg", "_L.jpg"),
                            })
                        )
                    );
                } else {
                    imageInfo.append(
                        $("<img />", {
                            class: "",
                            loading: "lazy",
                            onError: "this.src='/img/list_noimg.jpg'",
                            src:
                                item.img_file_name === null
                                    ? "/img/list_noimg.jpg"
                                    : $.cardUtils.getImgFolder(
                                        item.auc_kind,
                                        item.auc_num,
                                        item.auc_kind === "1" ? "/T" : ""
                                    ) +
                                    "/" +
                                    item.img_file_name.replace(".jpg", "_L.jpg"),
                        })
                    );
                }

                // Card > WorkInfo
                var artist = $.commonUtils.isNull(item.birth_date)
                    ? item.artist_name
                    : item.artist_name + " (" + item.birth_date + ")";

                let etc = item.material + item.display_edition;
                etc += "<br/>";
                etc +=
                    item.size + (item.make_date === "" ? "" : " | " + item.make_date);

                var workInfo = $("<div />", { class: "card-list-info" }).append(
                    $("<div />", { class: "card-lot-num" }).append("Lot. " + item.lot_num),
                    $("<h5 />", { class: "card-writer" }).append(artist),
                    $("<h5 />", { class: "card-subtitle-desc" }).append(
                        item.title
                    ),
                    $("<h5 />", { class: "card-subtitle-infodesc" }).append(etc)
                );

                // Card > AucInfo card-price-list
                var aucInfo = $("<div />", { class: "card-price-info" }).append(
                    $("<div />", { class: "card-price-list" }).append(
                        $("<ul />", { class: "price-list" }).append(
                            $("<li />").append(
                                ka.msg.auction.startingPrice
                            ),
                            $("<li />").append("KRW " + $.stringUtils.comma(item.price_start))
                        ),
                        $("<ul />", { class: "price-list" }).append(
                            $("<li />").append(
                                ka.msg.auction.bidPrice
                            ),
                            $("<li />", {
                                class: "pull-right text-right text-primary",
                            }).append(
                                (item.nak_yn === "Y"
                                    ? "[" + ka.msg.auction.winningBid + "] KRW "
                                    : "KRW ") + $.stringUtils.comma(item.price_bid)
                            )
                        ),
                        $("<ul />", { class: "price-list" }).append(
                            $("<li />").append(
                                item.end_yn === "Y"
                                    ? ka.msg.main.current
                                    : ka.msg.auction.hammerPrice
                            ),
                            $("<li />", {
                                class: "pull-right text-right",
                            }).append(
                                "KRW " +
                                (item.end_yn === "Y"
                                    ? $.stringUtils.comma(item.price_max)
                                    : $.stringUtils.comma(item.price_hammer))
                            )
                        ),
                        $("<ul />", { class: "price-list" }).append(
                            $("<li />").append(
                                ka.msg.auction.bidDate
                            ),
                            $("<li />", {
                                class: "pull-right text-right",
                            }).append(item.bid_reg_date)
                        )
                    )
                );

                if (item.auc_kind === "1") {
                    aucInfo.append(
                        $("<ul />", { class: "price-list" }).append(
                            $("<li />").append(
                                ka.msg.auction.bidPrice
                            ),
                            $("<li />", {
                                class: "pull-right text-right text-primary",
                            }).append(
                                item.major_bid_type_name +
                                " KRW " +
                                $.stringUtils.comma(item.price_bid)
                            )
                        ),
                        $("<ul />", { class: "price-list" }).append(
                            $("<li />").append(
                                ka.msg.auction.progress
                            ),
                            $("<li />", {
                                class: "pull-right text-right text-primary",
                            }).append(item.major_bp_state_name)
                        )
                    );
                } else {
                    if (successfulBid === "N") {
                        aucInfo.append(
                            $("<a />", {
                                class:
                                    "btn btn-outline-black btn-lg btn-block btn-icon-right m-t-25",
                                onclick: "$.mypageUtils.openMyBidList(" + item.uid + ")",
                            }).append(
                                "<span>" +
                                ka.msg.list.bidStatus +
                                "</span><i class='far fa-arrow-right p-r-10'></i>"
                            )
                        );
                    } else {
                        let certEl = null;
                        if (item.certificate_yn === "Y") {
                            // 보증서 조건을 우선으로 처리
                            if (
                                item.certificate_delivery_state !== null &&
                                item.certificate_delivery_state === "001"
                            ) {
                                certEl = $("<ul />", { class: "price-list" }).append(
                                    $("<li />").append(),
                                    $("<li />", {
                                        class:
                                            "list-inline-item pull-right text-right guarantee_print_btn",
                                    }).append(
                                        "<span class='hover_underline' style='cursor: pointer; line-height: 1.2em; display: inline-block; font-size: 13px;' onclick='certificateAlert(\"R\");'>" +
                                        ka.msg.mypage.certificateReqDate +
                                        "<br >" +
                                        item.certificate_delivery_reg_date +
                                        "</span>"
                                    )
                                );
                            } else {
                                if (item.certificate_print_yn === "Y") {
                                    certEl = $("<ul />", { class: "list-inline" }).append(
                                        $("<li />", { class: "list-inline-item" }).append(),
                                        // $('<li />', { "class": "list-inline-item pull-right text-right guarantee_print_btn" }).append("<a style='cursor: pointer;' onclick='modalReport(" + item.uid + ", \"" + item.certificate_yn + "\");'>" + ka.msg.successfulBid.certificate + "</a>")
                                        $("<li />", {
                                            class:
                                                "list-inline-item pull-right text-right guarantee_print_btn",
                                        }).append(
                                            "<span class='hover_underline' style='cursor: pointer;' onclick='certificateAlert(\"C\");'>" +
                                            ka.msg.mypage.certificatePrintDate +
                                            "<br >" +
                                            item.certificate_print_date +
                                            "</span>"
                                        )
                                    );
                                } else {
                                    certEl = $("<ul />", { class: "list-inline" }).append(
                                        $("<li />", { class: "list-inline-item" }).append(),
                                        $("<li />", {
                                            class:
                                                "list-inline-item pull-right text-right guarantee_print_btn",
                                        }).append(
                                            "<a style='cursor: pointer;' onclick='modalReport(" +
                                            item.ow_uid +
                                            ', "' +
                                            item.certificate_yn +
                                            "\");'>" +
                                            ka.msg.successfulBid.certificate +
                                            "</a>"
                                        )
                                    );
                                }
                            }
                        } else {
                            // 케이오피스 출력 완료
                            if (item.certificate_print_yn === "Y") {
                                certEl = $("<ul />", { class: "list-inline" }).append(
                                    $("<li />", { class: "list-inline-item" }).append(),
                                    $("<li />", {
                                        class:
                                            "list-inline-item pull-right text-right guarantee_print_btn",
                                    }).append(
                                        "<span class='hover_underline' style='cursor: pointer;' onclick='certificateAlert(\"C\");'>" +
                                        ka.msg.mypage.certificatePrintDate +
                                        "<br >" +
                                        item.certificate_print_date +
                                        "</span>"
                                    )
                                );
                            }
                        }

                        aucInfo.append(
                            $("<ul />", { class: "list-inline" }).append(
                                $("<li />", { class: "list-inline-item" }).append(
                                    ka.msg.auction.purchasePrice
                                ),
                                $("<li />", {
                                    class: "list-inline-item pull-right text-right text-primary",
                                }).append("KRW " + $.stringUtils.comma(item.price_purchase))
                            ),
                            //(item.certificate_yn === 'N' || item.certificate_print_yn === 'Y' ? null : $('<ul />', { "class": "list-inline" }).append(
                            //    $('<li />', { "class": "list-inline-item" }).append(),
                            //    $('<li />', { "class": "list-inline-item pull-right text-right guarantee_print_btn" }).append("<a style='cursor: pointer;' onclick='modalReport(" + item.uid + ", \"" + item.certificate_yn + "\");'>" + ka.msg.successfulBid.certificate + "</a>")
                            //))
                            certEl
                        );
                    }
                }

                card.append(imageInfo, workInfo, aucInfo);

                $(target).append(container.append(card));
            },

            /*------------------------------------------------------------------
              * @function:    setSuccessfulBidInfoCardTag
              * @param:       target (태그를 붙일 상위 노드)
              *               totalPrice (결제 금액)
              *               fees (수수료)
              * @description: 낙찰 작품 리스트 Card 형식의 object 생성하여 target 에 추가
              ------------------------------------------------------------------*/
            setSuccessfulBidInfoCardTag: function (
                target,
                totalPrice,
                fees,
                aucKind,
                aucNum
            ) {
                $(target).append(
                    $("<ul />", { class: "list-inline" }).append(
                        $("<li />", { class: "list-inline-item" }).append(
                            $("<div />").append(ka.msg.list.premium),
                            // $('<div />').append(typeof item !== "undefined" && item.price_hammer > 10000000 ? ka.msg.list.premium2_1.replace("{Fee}", (fees * 100).toString()) : ka.msg.list.premium2.replace("{Fee}", (fees * 100).toString())),
                            $("<div />").append(
                                ka.msg.list.premium3.replace(
                                    "{Price}",
                                    $.stringUtils.comma(totalPrice)
                                )
                            )
                        ),
                        $("<li />", { class: "list-inline-item pull-right" }).append(
                            $("<a />", {
                                class: "btn btn-outline-black btn-lg btn-block btn-icon-right",
                                onclick:
                                    "javascript:GetBidNotification(" +
                                    aucKind +
                                    "," +
                                    aucNum +
                                    ");",
                            }).append("<span>" + ka.msg.list.viewWinninBidResult + "</span>")
                        )
                    )
                );
            },

            /*------------------------------------------------------------------
              * @function:    getDataTag
              * @param:       type (표기유형)
              *               item (아이템)
              *               primary (강조여부)
              * @description: 카드 우측 항목별 보여질 Tag 리턴
              ------------------------------------------------------------------*/
            getDataTag: function (type, item, primary, loginCheck) {
                const loginFlag =
                    typeof login === "boolean" ? loginCheck : item.is_login;

                var header;
                var data;
                var currency = item.hongkong_yn === "Y" ? "HKD" : "KRW";
                switch (type) {
                    case "current":
                        header = ka.msg.main.current;
                        data =
                            currency +
                            " " +
                            (item.price_max > 0 ? $.stringUtils.comma(item.price_max) : "-");
                        break;
                    case "estimate":
                        header =
                            item.disp_type !== "R" ||
                                item.admin_yn === "Y" ||
                                item.active_yn === "Y"
                                ? ka.msg.auction.estimate
                                : "";
                        data =
                            item.disp_type !== "R" ||
                                item.admin_yn === "Y" ||
                                item.active_yn === "Y"
                                ? item.separate_inquiry_yn === "Y"
                                    ? ka.msg.auction.separateInquiryYN
                                    : currency +
                                    " " +
                                    $.stringUtils.comma(item.price_estimated_low) +
                                    " ~ " +
                                    $.stringUtils.comma(item.price_estimated_high)
                                : "";
                        break;
                    case "hammer":
                        header =
                            item.disp_type !== "R" ||
                                item.admin_yn === "Y" ||
                                item.active_yn === "Y"
                                ? ka.msg.auction.hammer
                                : "";
                        data =
                            item.disp_type !== "R" ||
                                item.admin_yn === "Y" ||
                                item.active_yn === "Y"
                                ? loginFlag
                                    ? currency + " " + $.stringUtils.comma(item.price_hammer)
                                    : "<a class='text-primary' href='javascript:$.commonUtils.openLogin(\"" +
                                    ka.msg.auction.bidListCheck +
                                    "\");'>" +
                                    ka.msg.auction.bidListCheck +
                                    "</a>"
                                : "";
                        break;
                    case "starting":
                        header =
                            item.disp_type !== "R" ||
                                item.admin_yn === "Y" ||
                                item.active_yn === "Y"
                                ? ka.msg.auction.starting
                                : "";
                        data =
                            item.disp_type !== "R" ||
                                item.admin_yn === "Y" ||
                                item.active_yn === "Y"
                                ? currency + " " + $.stringUtils.comma(item.price_start)
                                : "";
                        break;
                    case "endTime":
                        header = ka.msg.auction.endTime;
                        data = item.end_time;
                        break;
                    case "endTimeHorizonal":
                        header = ka.msg.auction.endTime;
                        data = item.horizonal_end_time;
                        break;
                    case "bidCount":
                        header = ka.msg.auction.bidCount;
                        data = item.bid_cnt;
                        break;
                    case "wishRegDate":
                        header = ka.msg.auction.regDate;
                        data = item.wish_reg_date;
                        break;
                }

                return $("<ul />", { class: "price-list" }).append(
                    header !== "" ? $("<li />").append(header) : null,
                    data !== ""
                        ? $("<li />", {
                            class:
                                typeof primary === "string" && primary === "Y"
                                    ? "current"
                                    : "",
                        }).append(data)
                        : null
                );
            },

            /*------------------------------------------------------------------
              * @function:    getGoToBidLink
              * @param:       item (AuctionWork Object)
              * @description: 진행 중인 경매 목록 Url 리턴
              ------------------------------------------------------------------*/
            getGoToAuction: function (item) {
                var auction =
                    item.auc_kind === "1"
                        ? "Major/"
                        : item.auc_kind === "2"
                            ? "Premium/"
                            : "Weekly/";
                return "/Auction/" + auction + item.auc_num.toString();
            },

            /*------------------------------------------------------------------
              * @function:    getGoToBidLink
              * @param:       item (AuctionWork Object)
              * @description: 진행 중인 경매 작품 Url 리턴
              ------------------------------------------------------------------*/
            getGoToBidLink: function (item) {
                var auction =
                    item.auc_kind === "1"
                        ? "Major/"
                        : item.auc_kind === "2"
                            ? "Premium/"
                            : "Weekly/";
                return "/Auction/" + auction + item.auc_num + "/" + item.uid;
            },

            /*------------------------------------------------------------------
              * @function:    getAuctionTitle
              * @param:       value (aucKind)
              * @description: 경매명 리턴
              ------------------------------------------------------------------*/
            getAuctionTitle: function (value) {
                switch (value.toLowerCase()) {
                    case "1":
                        return "Major";
                    case "2":
                        return "Premium";
                    case "4":
                        return "Weekly";
                    default:
                        return "";
                }
            },
        }),
        ($.paginationUtils = {
            data: {
                target: null,
                totalCount: null,
                pageSize: 10,
                pageBlock: 10,
                page: 1,
                callback: null,
                parameters: null,
                back: false,
            },

            totalPage: 0,
            startPage: 0,
            endPage: 0,

            isLastPage: function () {
                return (
                    $.paginationUtils.data.page.toString() ===
                    $.paginationUtils.totalPage.toString()
                );
            },

            init: function (opt) {
                if (typeof opt === "undefined") return false;

                var oldparameters = $.paginationUtils.data.parameters;

                $.paginationUtils.data = $.extend({}, $.paginationUtils.data, opt);

                var option = $.paginationUtils.data;

                $.paginationUtils.totalPage =
                    option.totalCount % option.pageSize !== 0
                        ? parseInt(option.totalCount / option.pageSize, 10) + 1
                        : parseInt(option.totalCount / option.pageSize, 10);
                $.paginationUtils.startPage =
                    option.page % option.pageBlock !== 0
                        ? parseInt(option.page / option.pageBlock, 10) * option.pageBlock +
                        1
                        : parseInt(option.page / option.pageBlock - 1, 10) *
                        option.pageBlock +
                        1;
                $.paginationUtils.endPage =
                    $.paginationUtils.startPage + option.pageBlock - 1 <
                        $.paginationUtils.totalPage
                        ? ($.paginationUtils.endPage =
                            $.paginationUtils.startPage + option.pageBlock - 1)
                        : ($.paginationUtils.endPage = $.paginationUtils.totalPage);

                if (option.target !== null && !isNaN($.paginationUtils.totalPage)) {
                    option.target.empty();

                    var li, a;
                    li = $("<li />", {
                        class:
                            "paginate_button page-item first " +
                            (option.page === 1 ? "disabled" : ""),
                    });
                    a = $("<a />", { class: "page-link" });
                    if (option.page !== 1) {
                        a.attr("href", "javascript:$.paginationUtils.goPage(1);");
                    }
                    option.target.append(
                        li.append(
                            a.append(
                                $("<i />", { class: "far fa-chevron-double-left fa-fw fs-9" })
                            )
                        )
                    );

                    li = $("<li />", {
                        class:
                            "paginate_button page-item previous " +
                            (option.page <= option.pageBlock ? "disabled" : ""),
                    });
                    a = $("<a />", { class: "page-link" });
                    if (option.page > option.pageBlock) {
                        a.attr(
                            "href",
                            "javascript:$.paginationUtils.goPage(" +
                            ($.paginationUtils.startPage - 1).toString() +
                            ");"
                        );
                    }
                    option.target.append(
                        li.append(
                            a.append($("<i />", { class: "far fa-chevron-left fa-fw fs-9" }))
                        )
                    );

                    for (
                        var i = $.paginationUtils.startPage;
                        i <= $.paginationUtils.endPage;
                        i++
                    ) {
                        li = $("<li />", {
                            class:
                                "paginate_button page-item " +
                                (option.page.toString() === i.toString() ? "active" : ""),
                        });
                        a = $("<a />", { class: "page-link" }).append(i.toString());
                        if (option.page !== i) {
                            a.attr("href", "javascript:$.paginationUtils.goPage(" + i + ");");
                        }
                        option.target.append(li.append(a));
                    }

                    li = $("<li />", {
                        class:
                            "paginate_button page-item next " +
                            ($.paginationUtils.endPage >= $.paginationUtils.totalPage
                                ? "disabled"
                                : ""),
                    });
                    a = $("<a />", { class: "page-link" });
                    if ($.paginationUtils.endPage < $.paginationUtils.totalPage) {
                        a.attr(
                            "href",
                            "javascript:$.paginationUtils.goPage(" +
                            ($.paginationUtils.endPage + 1) +
                            ");"
                        );
                    }
                    option.target.append(
                        li.append(
                            a.append($("<i />", { class: "far fa-chevron-right fa-fw fs-9" }))
                        )
                    );

                    li = $("<li />", {
                        class:
                            "paginate_button page-item last " +
                            (option.page === $.paginationUtils.totalPage ? "disabled" : ""),
                    });
                    a = $("<a />", { class: "page-link" });
                    if (option.page !== $.paginationUtils.totalPage) {
                        a.attr(
                            "href",
                            "javascript:$.paginationUtils.goPage(" +
                            $.paginationUtils.totalPage +
                            ");"
                        );
                    }
                    option.target.append(
                        li.append(
                            a.append(
                                $("<i />", { class: "far fa-chevron-double-right fa-fw fs-9" })
                            )
                        )
                    );
                }
            },

            reset: function (pageBlock) {
                $.paginationUtils.data.pageBlock = pageBlock;
                $.paginationUtils.init();
            },

            goPage: function (page) {
                var option = $.paginationUtils.data;
                if (option.callback !== null && typeof option.callback === "function") {
                    option.page = page;
                    // option.parameters["back"] = false;
                    if (option.parameters !== null) {
                        option.parameters.page = page;
                    }
                    //option.callback.call();
                    //$.paginationUtils.initPaintHTML($.paginationUtils.data.parameters);
                    //if (document.querySelector(".section-align") !== null) {
                    //    var menuHeight = document.querySelector(".section-align").offsetTop;
                    //    $("html, body").animate({ scrollTop: menuHeight + 60 }, "smooth");
                    //} else {
                    //    $("html, body").animate({ scrollTop: 0 }, "smooth");
                    //}
                    window.location.href =
                        window.location.pathname +
                        $.stringUtils.jsonToQueryString(option.parameters);
                }
            },

            initPaintHTML: function (initFormData) {
                if (initFormData) {
                    history.pushState({ formData: initFormData }, null, "");
                }
            },

            paintHTML: function (formData) {
                try {
                    requestParam = formData;
                    requestParam["back"] = true;
                    getList();
                } catch (e) { }
            },
        }),
        ($.loginUtils = {
            captchaMode: false,
            captchaCode: "",

            proc: function () {
                var id = $("#modal-login-id");
                var pwd = $("#modal-login-pwd");
                var captcha = $("#modal-login-captcha");
                var isSaved = $("#check-login").is(":checked");

                $("#validation-message").html("");

                if (id.val().replace(/ /gi, "") === "") {
                    $("#validation-message").html(ka.msg.join.idEmpty);
                    return false;
                } else if (pwd.val().replace(/ /gi, "") === "") {
                    $("#validation-message").html(ka.msg.join.pwdEmpty);
                    return false;
                } else if (
                    $.loginUtils.captchaMode &&
                    captcha.val().replace(/ /gi, "") === ""
                ) {
                    $("#validation-captcha-message").html(ka.msg.login.reqCaptcha);
                    $("#validation-captcha-message").addClass("text-primary");
                    captcha.val("");
                    return false;
                } else if (
                    $.loginUtils.captchaMode &&
                    $.loginUtils.captchaCode !== captcha.val().replace(/ /gi, "")
                ) {
                    $("#validation-captcha-message").html(ka.msg.login.notMatchCaptcha);
                    $("#validation-captcha-message").addClass("text-primary");
                    $.loginUtils.createCaptcha("login-captcha");
                    captcha.val("");
                    return false;
                } else {
                    var param = {};
                    param["id"] = id.val();
                    param["pwd"] = pwd.val();
                    param["is_saved"] = isSaved ? "T" : "F";
                    param["highlight_read"] = decodeURI(
                        $.commonUtils.getCookie("ka-highlight-read")
                    );
                    var result = $.ajaxUtils.getApiData(
                        "/api/Member/Login",
                        param,
                        null,
                        false
                    );
                    try {
                        captcha.val("");
                        if (result.code === "00") {
                            window.location.reload();
                        } else if (result.code === "00P") {
                            $("#modal-login").click();
                            $.commonUtils.confirmLogin(
                                ka.msg.auction.Notice,
                                ka.msg.main.changePwdMsg,
                                "location.href='/Member/MyPage';"
                            );
                        } else if (result.code === "00Y") {
                            window.location.href = "/Member/MyPage";
                        } else if (result.code.indexOf("01") === 0) {
                            window.location.href =
                                "/Member/Agreement/" + result.code.split("|")[1];
                        } else if (result.code.indexOf("02") === 0) {
                            window.location.href = "/Member/Activation";
                        } else if (result.code.indexOf("ka.") === 0) {
                            $("#validation-message").html(eval(result.code));
                            if (
                                typeof result.data.fail_cnt !== "undefined" &&
                                typeof result.data.fail_cnt === "number"
                            ) {
                                if (result.data.fail_cnt >= 10) {
                                    $("#validation-message").html(
                                        ka.msg.login.fail10.replace(
                                            "{LockTime}",
                                            result.data.fail_lock_time.toString()
                                        )
                                    );
                                } else if (result.data.fail_cnt >= 5) {
                                    $.loginUtils.createCaptcha("login-captcha");
                                    $("#login-captcha-form").show();
                                    $("#validation-message").html(
                                        result.data.fail_cnt >= 8
                                            ? ka.msg.login.fail8
                                                .replace("{Num}", result.data.fail_cnt.toString())
                                                .replace(
                                                    "{LockTime}",
                                                    result.data.fail_lock_time.toString()
                                                )
                                            : ka.msg.login.fail5.replace(
                                                "{Num}",
                                                result.data.fail_cnt.toString()
                                            )
                                    );
                                    $("#validation-captcha-message").html(
                                        ka.msg.login.defaultMsg
                                    );
                                    $("#validation-captcha-message").attr(
                                        "class",
                                        "text-center m-b-10"
                                    );
                                } else if (result.data.fail_cnt >= 3) {
                                    $("#validation-message").html(
                                        ka.msg.login.fail3.replace(
                                            "{Num}",
                                            result.data.fail_cnt.toString()
                                        )
                                    );
                                }
                            }
                        } else {
                            $("#validation-message").html(result.message);
                        }
                        pwd.val("");
                    } catch (e) {
                        console.log(e.description);
                    }
                }
            },

            isLogin: function () {
                var result = $.ajaxUtils.getApiData(
                    "/api/Member/LoginCheck",
                    null,
                    null,
                    false
                );
                try {
                    return result.code === "00" ? true : false;
                } catch (e) {
                    console.log(e.description);
                    return false;
                }
            },

            redirectCheck: function (url, msg) {
                if ($.loginUtils.isLogin()) {
                    window.location.href = url;
                } else {
                    $.commonUtils.openLogin(msg);
                }
            },

            createCaptcha: function (target) {
                $("#" + target).html("");

                var charsArray =
                    "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                var lengthOtp = 6;
                var captcha = [];
                for (var i = 0; i < lengthOtp; i++) {
                    var index = Math.floor(Math.random() * charsArray.length + 1);
                    if (captcha.indexOf(charsArray[index]) == -1)
                        captcha.push(charsArray[index]);
                    else i--;
                }
                var canv = document.createElement("canvas");
                canv.id = "login_captcha";
                canv.width = 110;
                canv.height = 30;
                var ctx = canv.getContext("2d");
                ctx.font = "25px Georgia";
                ctx.textAlign = "center";
                ctx.strokeText(captcha.join(""), canv.width / 2, 20);
                $.loginUtils.captchaCode = captcha.join("");
                $("#" + target).html(canv);

                var refresh = $("<i />", {
                    class: "fal fa-sync",
                    style:
                        "vertical-align: top; margin-top: 5px; margin-left: 0px; cursor: pointer;",
                    onclick: "$.loginUtils.createCaptcha('" + target + "');",
                });
                $("#" + target).append(refresh);

                $.loginUtils.captchaMode = true;
            },

            updateRecevingAdvertising: function () {
                var paramAdv = {};
                paramAdv["receive_sms_info"] = $("#receive-ad-chk_01").is(":checked")
                    ? "Y"
                    : "N";
                paramAdv["receive_email_info"] = $("#receive-ad-chk_02").is(":checked")
                    ? "Y"
                    : "N";
                paramAdv["receive_phone_info"] = $("#receive-ad-chk_03").is(":checked")
                    ? "Y"
                    : "N";
                var result = $.ajaxUtils.getApiData(
                    "/api/Member/SetReceiveAdvertising",
                    paramAdv,
                    null,
                    false
                );
                $("#dormant-account-modal").modal("hide");
                if ($.ajaxUtils.getResultCode(result) !== "00") {
                    $.commonUtils.alert(ka.msg.common.error);
                }
            },

            setActivation: function () {
                var paramActivation = {};
                paramActivation["info_validate_period"] = $(
                    'input[name="dormant-account_chk"]:checked'
                ).val();
                paramActivation["receive_sms_info"] = $("#receive-ad-chk_01").is(
                    ":checked"
                )
                    ? "Y"
                    : "N";
                paramActivation["receive_email_info"] = $("#receive-ad-chk_02").is(
                    ":checked"
                )
                    ? "Y"
                    : "N";
                paramActivation["receive_phone_info"] = $("#receive-ad-chk_03").is(
                    ":checked"
                )
                    ? "Y"
                    : "N";
                var result = $.ajaxUtils.getApiData(
                    "/api/Member/SetActivation",
                    paramActivation,
                    null,
                    false
                );
                if ($.ajaxUtils.getResultCode(result) !== "00") {
                    $.commonUtils.alert(ka.msg.common.error);
                } else {
                    location.href = "/";
                }
            },
        }),
        ($.storageUtils = {
            storage: null,
            storageType: null,

            // type : session / local
            init: function (type) {
                $.storageUtils.storageType = type + "Storage";
                return $.storageUtils.isAvailable();
            },

            isAvailable: function () {
                try {
                    $.storageUtils.storage = window[$.storageUtils.storageType];
                    var x = "__storage_test__";
                    $.storageUtils.storage.setItem(x, x);
                    $.storageUtils.storage.removeItem(x);
                    return true;
                } catch (e) {
                    return (
                        e instanceof DOMException &&
                        // Firefox를 제외한 모든 브라우저
                        (e.code === 22 ||
                            // Firefox
                            e.code === 1014 ||
                            // 코드가 존재하지 않을 수도 있기 떄문에 이름 필드도 확인합니다.
                            // Firefox를 제외한 모든 브라우저
                            e.name === "QuotaExceededError" ||
                            // Firefox
                            e.name === "NS_ERROR_DOM_QUOTA_REACHED") &&
                        // 이미 저장된 것이있는 경우에만 QuotaExceededError를 확인하십시오.
                        storage &&
                        storage.length !== 0
                    );
                }
            },

            setItem: function (name, obj) {
                if ($.storageUtils.isAvailable()) {
                    $.storageUtils.storage.setItem(name, obj);
                }
            },

            removeItem: function (name) {
                if ($.storageUtils.isAvailable()) {
                    $.storageUtils.storage.removeItem(name);
                }
            },

            getItem: function (name) {
                if ($.storageUtils.isAvailable()) {
                    return $.storageUtils.storage.getItem(name);
                }
            },
        }),
        ($.eventUtils = {
            /*------------------------------------------------------------------
              * @function:    enter
              * @param:       event
              *               func (function 명)
              * @description: enter 이벤트시 func 파라미터의 값이 함수일 경우 호출 처리
              ------------------------------------------------------------------*/
            enter: function (event, func) {
                if (event.keyCode === 13 && typeof func === "function") {
                    func.call();
                }
            },
        }),
        ($.searchUtils = {
            setSearchKey: function (type, key, listMode) {
                var searchParam = {};
                searchParam["type"] = type;
                searchParam["key"] = key;
                $.ajaxUtils.getApiData(
                    "/api/Auction/WorkSearchHst",
                    searchParam,
                    null,
                    false
                );

                if (listMode && listMode === true) {
                    $.searchUtils.getSearchHistory();
                }
            },

            clearSearchHistory: function () {
                $.ajaxUtils.getApiData(
                    "/api/Home/DelAllSearchHistory",
                    null,
                    null,
                    false
                );
                $.searchUtils.getSearchHistory();
            },

            delSearchHistory: function (key) {
                var searchParam = {};
                // searchParam["uid"] = uid;
                searchParam["key"] = key;
                $.ajaxUtils.getApiData(
                    "/api/Home/DelSearchHistory",
                    searchParam,
                    null,
                    false
                );
                $.searchUtils.getSearchHistory();
            },

            getSearchHistory: function () {
                $.ajaxUtils.getApiData(
                    "/api/Home/GetSearchHistories",
                    null,
                    $.searchUtils.getSearchHistoryComplete
                );
            },

            getSearchHistoryComplete: function (result) {
                if ($.ajaxUtils.getResultCode(result) === "00") {
                    // 상단 유틸메뉴
                    $("#search-history-cont, #search-history-cont-mobile").empty();
                    let isEmpty = true;
                    $.each(result.data, function (index, item) {
                        isEmpty = false;
                        let el = "<div>";
                        el +=
                            "<span style='cursor: pointer;' onclick='window.location.href=\"/Home/Search?key=" +
                            encodeURI(item.key) +
                            "\"'>" +
                            item.key +
                            "</span>";
                        // el += "<button type='button' text=''><i class='fas fa-times' onclick='$.searchUtils.delSearchHistory(" + item.uid + ");'></i></button>";
                        el +=
                            '<button type="button" text=""><i class="fas fa-times" onclick="$.searchUtils.delSearchHistory(\'' +
                            item.key +
                            "');\"></i></button>";
                        el += "</div>";
                        $("#search-history-cont, #search-history-cont-mobile").append(el);
                    });

                    if (isEmpty) {
                        $("#search-history-cont, #search-history-cont-mobile").append(
                            "<div class='history-empty-cnt'><span>" +
                            ka.msg.common.nothingDisplay +
                            "</span></div>"
                        );
                        $(".btnSearchInit").hide();
                    }

                    // 검색 페이지 입력 폼 하단 레이어
                    if ($("#search-history-layer").length > 0) {
                        $("#search-history-layer").empty();
                        $.each(result.data, function (index, item) {
                            let el = "<li>";
                            el += "<i class='far fa-clock'></i>";
                            el +=
                                "<span style='cursor: pointer;' onclick='window.location.href=\"/Home/Search?key=" +
                                encodeURI(item.key) +
                                "\"'>" +
                                item.key +
                                "</span>";
                            // el += "<div class='search-list-close' onclick='$.searchUtils.delSearchHistory(" + item.uid + ");'>";
                            el +=
                                '<div class="search-list-close" onclick="$.searchUtils.delSearchHistory(\'' +
                                item.key +
                                "');\">";
                            el +=
                                "<img src='/img/icons/search_close@1x.png' alt='close-btn' srcset='/img/icons/search_close@1x.png 1x, /img/icons/search_close@2x.png 2x, /img/icons/search_close@3x.png 3x' />";
                            el += "</div>";
                            el += "</li>";
                            $("#search-history-layer").append(el);
                        });
                    }
                }
            },

            getSelectedSearchOptionLabelTag: function (type, value, display) {
                return (
                    '<div class="selected-search-list"><span>' +
                    display +
                    '</span><button data-type="' +
                    type +
                    '" data-value="' +
                    value +
                    '" onclick="$.searchUtils.removeSelectedSearchOption(this);"><i class="fas fa-times"></i></button></div>'
                );
            },

            removeSelectedSearchOption: function (obj) {
                var type = obj.dataset.type;
                var value = obj.dataset.value;

                let orgValue = requestParam[type];

                if (type === "artist") {
                    let newValue = "";
                    $.each(orgValue.split(","), function (index, item) {
                        if (item.toString() !== value.toString()) {
                            newValue += newValue === "" ? item : "," + item;
                        }
                    });
                    requestParam[type] = newValue;
                } else if (
                    type === "price_start" ||
                    type === "price_estimated_low" ||
                    type === "price_estimated_high"
                ) {
                    delete requestParam[type + "_from"];
                    delete requestParam[type + "_to"];
                } else if (type === "search") {
                    delete requestParam["key"];
                    delete requestParam["search"];
                } else if (type === "search_a") {
                    delete requestParam["key"];
                    delete requestParam["search"];
                    delete requestParam["filter"];
                } else if (type === "auction_range") {
                    if (value === "P") {
                        if ($(".auction-range-L").is(":checked")) {
                            requestParam[type] = "L";
                        } else {
                            // delete requestParam[type];
                            requestParam[type] = "E";
                        }
                    } else if (value === "L") {
                        if ($(".auction-range-P").is(":checked")) {
                            requestParam[type] = "P";
                        } else {
                            // delete requestParam[type];
                            requestParam[type] = "E";
                        }
                    }
                } else if (type === "auc_kind") {
                    let selectedValue = "";
                    $.each($(".search-auc-kind-0"), function (index, item) {
                        if (item.checked) {
                            if (item.value.toString() !== value) {
                                selectedValue +=
                                    selectedValue !== "" ? "|" + item.value : item.value;
                            }
                        }
                    });
                    requestParam[type] = selectedValue === "" ? "E" : selectedValue;
                } else {
                    delete requestParam[type];
                }

                if (window.location.pathname.toLowerCase().indexOf("/search") > -1) {
                    delete requestParam["search"];
                }

                window.location.href =
                    window.location.pathname +
                    $.stringUtils.jsonToQueryString(requestParam);
            },
        }),
        ($.addressUtils = {
            formatCountry: function (country) {
                if (!country.id) {
                    return country.text;
                }
                var countryOption = country.element;
                return (
                    "<span class='m-r-10 m-b-1 flag flag-" +
                    $(countryOption).data("iso-alpha2").toLowerCase() +
                    "'></span><span class='text-uppercase'>" +
                    country.text +
                    "</span>"
                );
            },

            openAddrSearch: function () {
                $("#search_addr").val("");
                $("#addr_list").empty();
                $("#addr_list").css("display", "none");
                $("#join_member_address_modal").modal();
            },

            enterAddrSearch: function () {
                let evt_code = window.netscape ? ev.which : event.keyCode;
                if (evt_code === 13) {
                    let keyword = $("#search_addr").val();
                    if (keyword.replace(/ /gi, "") !== "") {
                        $("#addrKeyword").val(keyword);
                        $.addressUtils.getAddr();
                    }
                }
                return false;
            },

            addrSearch: function () {
                let keyword = $("#search_addr").val();

                if (keyword.replace(/ /gi, "") !== "") {
                    $("#addrKeyword").val(keyword);
                    event.keyCode = 0;
                    $.addressUtils.getAddr();
                }
            },

            getAddr: function () {
                $.ajax({
                    url: "https://www.juso.go.kr/addrlink/addrLinkApiJsonp.do",
                    type: "post",
                    data: $("#address-form").serialize(),
                    dataType: "jsonp",
                    crossDomain: true,
                    async: true,
                    success: function (jsonStr) {
                        try {
                            var errCode = jsonStr.results.common.errorCode;
                            var errDesc = jsonStr.results.common.errorMessage;
                            if (errCode != "0") {
                                console.log(errCode + "=" + errDesc);
                            } else {
                                if (jsonStr != null) {
                                    if (
                                        jsonStr.results &&
                                        jsonStr.results.juso &&
                                        jsonStr.results.juso.length < 1
                                    ) {
                                        $.commonUtils.alert(ka.msg.list.emptySeach);
                                        return false;
                                    }

                                    $("#addr_list").empty();
                                    $.addressUtils.makeListJson(jsonStr);
                                }
                            }
                        } catch (e) {
                            console.log(e.description);
                        }
                    },
                    error: function (xhr, status, error) {
                        console.log("에러발생");
                    },
                });
            },

            makeListJson: function (jsonStr) {
                let addrList = $("<ul />", { class: "w-100 addrList" });
                $.each(jsonStr.results.juso, function (index, item) {
                    $("<li >", {
                        onclick: "$.addressUtils.setAddrData(this);",
                        style: "cursor: pointer;",
                        "data-road-addr": item.roadAddr,
                        "data-jibun-addr": item.jibunAddr,
                        "data-zip-no": item.zipNo,
                    })
                        .append(
                            "<div><span class='new'>도로명</span> " +
                            item.roadAddr +
                            "</div>" +
                            "<div><span class='old'>지번</span> " +
                            item.jibunAddr +
                            "</div>"
                        )
                        .appendTo(addrList);
                });
                $("#join_member_address_modal .tip").css("display", "none");
                $("#addr_list").css("display", "block");
                $("#addr_list").append(addrList);
            },

            setAddrData: function (obj) {
                if ($("#modal-zipcode").length > 0) {
                    $("#modal-zipcode").val(obj.dataset.zipNo);
                } else {
                    $("#modal-zip-code").val(obj.dataset.zipNo);
                }
                $("#modal-address1").val(obj.dataset.roadAddr);
                $("#join_member_address_modal").modal("hide");

                if (obj.dataset.zipNo !== "") {
                    $("#zip-code").parent().find(".text_box_clearbtn").show();
                } else {
                    $("#zip-code").parent().find(".text_box_clearbtn").hide();
                }
            },
        }),
        ($.domUtils = {
            empty: function (target) {
                if (document.querySelectorAll(target)) {
                    const el = document.querySelectorAll(target);
                    el.forEach(function (item, index) {
                        while (item.firstChild) {
                            item.removeChild(item.firstChild);
                        }
                    });
                }
            },
        });
})(window.jQuery);
