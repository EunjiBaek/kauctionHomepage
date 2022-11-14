(function ($) {
    'use strict';

    $.commonUtils = {
        /*------------------------------------------------------------------
        * @function:    alert
        * @param:       value (메시지)
        *               type (success/error) - error 가 기본값
        * @description: 메시지의 내용을 모달 오픈
        ------------------------------------------------------------------*/
        alert: function (value, type) {
            if (typeof type === "undefined") type = "";

            var elType = $("#modal-type");
            elType.attr("class", "modal-icon animate");
            elType.empty();
            if (type === "success") {
                elType.addClass("modal-success");
                elType.append('<span class="modal-line modal-tip animateSuccessTip"></span>');
                elType.append('<span class="modal-line modal-long animateSuccessLong"></span>');
            } else {
                elType.addClass("modal-error");
                elType.append('<span class="modal-x-mark">');
                elType.append('<span class="modal-line modal-left animateXLeft"></span>');
                elType.append('<span class="modal-line modal-right animateXRight"></span>');
                elType.append('</span>');
            }
            elType.append('<div class="modal-placeholder"></div>');
            elType.append('<div class="modal-fix"></div>');

            var el = $("#modal-common");
            el.find("#modal-message").html(value);
            el.find('.btn-cstm-secondary').attr("onclick", "");
            el.modal('show');
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
                elType.addClass("modal-success");
                elType.append('<span class="modal-line modal-tip animateSuccessTip"></span>');
                elType.append('<span class="modal-line modal-long animateSuccessLong"></span>');
            } else {
                elType.addClass("modal-error");
                elType.append('<span class="modal-x-mark">');
                elType.append('<span class="modal-line modal-left animateXLeft"></span>');
                elType.append('<span class="modal-line modal-right animateXRight"></span>');
                elType.append('</span>');
            }
            elType.append('<div class="modal-placeholder"></div>');
            elType.append('<div class="modal-fix"></div>');

            var el = $("#modal-common");
            el.find("#modal-message").html(value);
            el.find('.btn-ok').attr("onclick", fn);
            el.modal('show');
        },

        /*------------------------------------------------------------------
        * @function:    confirm
        * @param:       title (타이틀)
        *               message (메시지)
        *               func (확인 클릭 시 호출 함수 값)
        * @description: 메시지의 내용을 Confirm 오픈
        ------------------------------------------------------------------*/
        confirm: function (title, message, func, ) {
            const target = "modal-warning";
            const el = $(`#${target}`);
            el.modal('hide');
            el.find(`#${target}-title`).html(title);
            el.find(`#${target}-message`).html(message);
            el.modal('show');

            $("#btn-warning-confirm").attr("onclick", func);
        },

        /*------------------------------------------------------------------
        * @function:    getRecruitFormTemplate
        * @description: 콘텐츠 관리 > 파일 업로드 > 채용 업로드 시 HTML 처리할 기본 HTML
        ------------------------------------------------------------------*/
        getRecruitFormTemplate: function () {
            return "<textarea class='form-control' rows='15'><div style='text-align: right; border-bottom: 1px solid #ddd; padding: 0 0 10px;'>"
                + "<a href='http://images.k-auction.com/AD/2020/케이옥션 입사지원서.doc' style='color: #3a5fcd; margin-right: 12px; font-weight: bold;' > 입사지원서 다운로드 </a>"
                + "</div>"
                + "<div style='text-align: center; margin-top: 24px;'>"
                + "<img src='{IMAGE}' alt='채용공고' />"
                + "</div>"
                + "</textarea>";
        },

        /*------------------------------------------------------------------
        * @function:    getPrFormTemplate
        * @description: 콘텐츠 관리 > 파일 업로드 > 홍보 업로드 시 HTML 처리할 기본 HTML
        ------------------------------------------------------------------*/
        getPrFormTemplate: function () {
            return "<textarea class='form-control' rows='15'>"
                + "<div align='center;' style='width: 800px; margin: 0 auto; margin-block-start: 0; margin-block-end: 0;'>"
                + "<p align='center' style='text-align:center; margin-block-start:0; margin-block-end:0;'>"
                + "<a href='[경매주소에 맞게 변경하세요.]'>"
                + "<img style='display:block;' src='{IMAGE}'>"
                + "</a>"
                + "</p>"
                + "</div>"
                + "</textarea>";
        }
    },

    $.stringUtils = {
        left: function (value, len) {
            if (len <= 0)
                return "";
            else if (len > String(value).length)
                return str;
            else
                return String(value).substring(0, length);
        },
        right: function (value, len) {
            if (len <= 0)
                return "";
            else if (len > String(value).length)
                return value;
            else {
                var iLen = String(value).length;
                return String(value).substring(iLen, iLen - len);
            }
        },
        comma: function (value) {
            return typeof value === "undefined" ? "" : value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        }
    },

    // 사용안하지만 남겨놓음
    $.listUtils = {
        language: {
            search: "<div class='form-group-default input-group'>_INPUT_<div class='input-group-append'><span class='input-group-text black'><i class='fas fa-search fa-fw'></i></span></div></div>",
            searchPlaceholder: "Search...",
            info: "<b>_TOTAL_</b> 건",
            infoEmpty: "0 건",
            infoFiltered: "/ 총 <b>_MAX_</b> 건 중",
            emptyTable: "<p class='p-t-50 p-b-50 hint-text'>등록된 게시물이 없습니다.</p>",
            zeroRecords: "<p class='p-t-50 p-b-50 hint-text'>검색 결과가 없습니다.</p>",
            processing: "<p class='p-t-50 p-b-50 hint-text'>처리중...</p>",
            paginate: {
                first: "<i class='far fa-chevron-double-left fa-fw fs-9'></i>",
                previous: "<i class='far fa-chevron-left fa-fw fs-9'></i>",
                next: "<i class='far fa-chevron-right fa-fw fs-9'></i>",
                last: "<i class='far fa-chevron-double-right fa-fw fs-9'></i>"
            }
        }
    },

    $.ajaxUtils = {
        /*------------------------------------------------------------------
        * @function:    setParameterString
        * @description: 태그의 속성 중 param 이 Y인 요소의 값을 json 처리
        * @returns:     json (element id/value) 리턴
        ------------------------------------------------------------------*/
        setParameter: function (param) {
            var target = ["select", "input", "textarea"];
            $.each(target, function (rootIndex, rootItem) {
                $.each($(rootItem), function (index, item) {
                    if (item.attributes["param"] !== undefined && item.attributes["param"].value === "Y") {
                        var targetID = typeof item.attributes["param-id"] === "undefined" ? item.id : item.attributes["param-id"].value;
                        if (targetID === "") return true;
                        if ($("#" + item.id).val() !== "" && (item.type === "hidden" || $("#" + item.id).css("display") !== "none")) {
                            switch (rootItem) {
                                case "select":
                                    param[targetID] = $("#" + item.id + " option:selected").val();
                                    break;
                                case "input":
                                    switch (item.type) {
                                        case "radio":
                                            param[targetID] = $("input[name='" + item.name + "']:checked").val();
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
            if (typeof parameter === "object") parameter = JSON.stringify(parameter);
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
                    if (typeof args === "string" && args.indexOf("<!DOCTYPE html>") > -1) {
                        alert("세션이 만료되었습니다.\n다시 로그인 하시기 바랍니다.");
                        window.location.href = "/?ReturnUrl=" + encodeURI(window.location.pathname);
                    }
                    if (async && typeof callback === "function") {
                        callback.call(this, args, parameter);
                    }

                    if (!async) { result = args; }
                },
                error: function (args) {

                },
                complete: function (args) {
                    // 로딩중 처리 종료
                }
            });

            return result;
        },

        /*------------------------------------------------------------------
        * @function:    getResultCode
        * @description: ajax 처리 결과 json 에서 결과 코드 값 확인
        * @returns:     결과 코드 리턴
        ------------------------------------------------------------------*/
        getResultCode: function (result) {
            return typeof result !== "undefined" && typeof result.code !== "undefined" ? result.code : "";
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
                    type: 'POST',
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
                    }
                });
            }
            return updateResult;
        },
    };

    $.treeViewUtils = {
        option: {
            url: null,
            param: null,
            target: null,
            onClick: null,
            id_field: null,
            display_field: null,
            disabled_field: null,
            selectedTaget: null,
        },

        init: function (option) {
            $.treeViewUtils.option = $.extend({}, $.treeViewUtils.option, option);

            var opt = $.treeViewUtils.option;
            if (opt.url !== null && opt.url !== "") {
                $.ajaxUtils.getApiData(opt.url, opt.param !== null ? opt.param : {}, $.treeViewUtils.initComplete);
            } else {
                console.log("url 이 지정되지 않았습니다.");
            }
        },

        initComplete: function (result) {
            if ($.ajaxUtils.getResultCode(result) === "00") {
                var opt = $.treeViewUtils.option;
                if (opt.target !== null) {
                    $(opt.target).empty();

                    var resultData = {};
                    var el = '<ul id="treeData" class="hidden">';
                    $.each(result.data, function (index, item) {
                        var disabled = item[opt.disabled_field] === "Y" ? 'style="text-decoration: line-through;"' : "";
                        el += '<li id="id_' + item[opt.id_field] + '" title="' + item[opt.display_field] + '" ' + disabled + '>' + item[opt.display_field] + '</li>';
                        resultData["id_" + item[opt.id_field]] = item;
                    });
                    el += '</ul>';
                    $(opt.target).html(el);

                    $(opt.target).dynatree({
                        fx: {
                            height: "toggle",
                            duration: 200
                        },
                        onSelect: function (select, node) { },
                        onClick: function (node, event) {
                            if (typeof opt.onClick === "function") {
                                opt.onClick.call(this, node, resultData[node.data.key]);
                            }
                        },
                        onActivate: function (node) {
                            if ($.treeViewUtils.option.selectedTarget !== null && $("#" + $.treeViewUtils.option.selectedTarget).length > 0) {
                                $("#" + $.treeViewUtils.option.selectedTarget).html(" - " + node.data.title);
                            }
                        },
                        classNames: {
                            active: "ui-dynatree-active"
                        }
                    });
                } else {
                    console.log("target 이 지정되지 않았습니다.");
                }
            } else {
                console.log("코드 데이터 로딩 중 오류가 발생했습니다.");
            }
        }
    },

    $.paginationUtils = {
        data: {
            target: null,
            totalCount: null,
            pageSize: 10,
            pageBlock: 10,
            page: 1,
            callback: null,
            parameters: null,
            moveTop: true
        },

        init: function (opt) {
            $.paginationUtils.data = $.extend({}, $.paginationUtils.data, opt);

            var option = $.paginationUtils.data;

            if (typeof isMobile === "string" && isMobile === "True") {
                option.pageBlock = 5;
            }

            var totalPage = option.totalCount % option.pageSize !== 0 ? parseInt(option.totalCount / option.pageSize, 10) + 1
                : parseInt(option.totalCount / option.pageSize, 10);
            var startPage = option.page % option.pageBlock !== 0 ? parseInt(option.page / option.pageBlock, 10) * option.pageBlock + 1
                : parseInt(option.page / option.pageBlock - 1, 10) * option.pageBlock + 1;
            var endPage = startPage + option.pageBlock - 1 < totalPage ? endPage = startPage + option.pageBlock - 1
                : endPage = totalPage;

            if (option.target !== null && !isNaN(totalPage)) {
                option.target.empty();

                var li, a;
                li = $("<li />", { "class": "paginate_button page-item first " + (option.page === 1 ? "disabled" : "") });
                a = $("<a />", { "class": "page-link" });
                if (option.page !== 1) {
                    a.attr("href", "javascript:$.paginationUtils.goPage(1);");
                }
                option.target.append(li.append(a.append($("<i />", { "class": "far fa-chevron-double-left fa-fw fs-9" }))));

                li = $("<li />", { "class": "paginate_button page-item previous " + (option.page <= option.pageBlock ? "disabled" : "") });
                a = $("<a />", { "class": "page-link" });
                if (option.page > option.pageBlock) {
                    a.attr("href", "javascript:$.paginationUtils.goPage(" + (startPage - 1).toString() + ");");
                }
                option.target.append(li.append(a.append($("<i />", { "class": "far fa-chevron-left fa-fw fs-9" }))));

                for (var i = startPage; i <= endPage; i++) {
                    li = $("<li />", { "class": "paginate_button page-item " + (option.page === i ? "active" : "") });
                    a = $("<a />", { "class": "page-link" }).append(i.toString());
                    if (option.page !== i) {
                        a.attr("href", "javascript:$.paginationUtils.goPage(" + i + ");");
                    }
                    option.target.append(li.append(a));
                }

                li = $("<li />", { "class": "paginate_button page-item next " + (endPage >= totalPage ? "disabled" : "") });
                a = $("<a />", { "class": "page-link" });
                if (endPage < totalPage) {
                    a.attr("href", "javascript:$.paginationUtils.goPage(" + (endPage + 1) + ");");
                }
                option.target.append(li.append(a.append($("<i />", { "class": "far fa-chevron-right fa-fw fs-9" }))));

                li = $("<li />", { "class": "paginate_button page-item last " + (option.page === totalPage ? "disabled" : "") });
                a = $("<a />", { "class": "page-link" });
                if (option.page !== totalPage) {
                    a.attr("href", "javascript:$.paginationUtils.goPage(" + totalPage + ");");
                }
                option.target.append(li.append(a.append($("<i />", { "class": "far fa-chevron-double-right fa-fw fs-9" }))));
            }
        },

        goPage: function (page) {
            var option = $.paginationUtils.data;
            if (option.callback !== null && typeof option.callback === "function") {
                option.page = page;
                if (option.parameters !== null) {
                    option.parameters.page = page;
                }
                option.callback.call();

                if ($.paginationUtils.data.moveTop) {
                    if (document.querySelector(".section-align") !== null) {
                        var menuHeight = document.querySelector(".section-align").offsetTop;
                        $("html, body").animate({ scrollTop: menuHeight }, "smooth");
                    } else {
                        $("html, body").animate({ scrollTop: 0 }, "smooth");
                    }
                }
            }
        }
    },

    $.datetimeUtils = {
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
            return today.getFullYear() + "-" + $.datetimeUtils.format(today.getMonth() + 1) + "-" + $.datetimeUtils.format(today.getDate());
        },

        /*------------------------------------------------------------------
        * @function:    dateDiff
        * @param:       orgDate (비교 날짜 개체)
        *               targetDate (비교 날짜 개체)
        * @description: 두개의 날짜를 비교하여 차이 일수를 리턴
        ------------------------------------------------------------------*/
        dateDiff: function (type, orgDate, targetDate) {
            var org = org instanceof Date ? orgDate : new Date(orgDate);
            var target = targetDate instanceof Date ? targetDate : new Date(targetDate);

            org = new Date(org.getFullYear(), org.getMonth() + 1, org.getDate(), org.getHours(), org.getMinutes(), org.getSeconds());
            target = new Date(target.getFullYear(), target.getMonth() + 1, target.getDate(), target.getHours(), target.getMinutes(), target.getSeconds());

            var diff = target - org;
            var timeGap = new Date(0, 0, 0, 0, 0, 0, target - org);

            if (type === 'D') {
                return Math.floor(diff / (1000 * 3600 * 24));
            } else if (type === 'H') {
                return timeGap.getHours();
            } else if (type === 'M') {
                return timeGap.getMinutes();
            } else if (type === 'S') {
                return timeGap.getSeconds();
            } else {
                return 0;
            }
        }
        },

    $.listViewUtils = {
        config: {
            target: null,
            totalCount: 0,
            data: null,
            parameters: null,
            callback: null,
            page: 1,
            pageSize: 20,
            pagination: null,
            totalRecord: null,
            fields: []
        },

        init: function (config) {
            $.extend($.listViewUtils.config, config);
            
            var _config = $.listViewUtils.config;
            try {
                _config.target.jsGrid({
                    autoload: true,
                    controller: {
                        loadData: function () {
                            if (_config.totalCount > 0) {
                                if (_config.totalRecord !== null) {
                                    $('#total-record').html('총 ' + _config.totalCount.toString() + '건');
                                }
                                if (_config.pagination !== null && _config.parameters !== null && _config.callback !== null) {
                                    $.paginationUtils.init({
                                        target: _config.pagination,
                                        totalCount: _config.totalCount,
                                        parameters: _config.parameters,
                                        callback: _config.callback,
                                        page: _config.page,
                                        pageSize: _config.pageSize
                                    });
                                }
                            }
                            return _config.data; // result.data.data;
                        }
                    },
                    fields: _config.fields,
                    //rowClick: function (args) {
                    //    window.open('/Member/' + args.item.uid.toString(), '_self');
                    //},
                    noDataContent: "검색 결과가 없습니다.",
                    loadMessage: "잠시만 기다려주세요...",
                    loadIndication: true,
                    loadIndicationDelay: 500,
                    loadShading: true
                });
            } catch (e) { console.log(e.description); }
        }
    }

})(window.jQuery);