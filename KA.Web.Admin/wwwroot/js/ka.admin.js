(function($) {
    'use strict';

    var KAuction = function() {
        this.scrollElement = 'html, body';
        this.$body = $('body');
        this.setUserOS();
        this.setUserAgent();
    }

    /*------------------------------------------------------------------
    * @function:    setUserOS
    * @description: 사용자 OD 설정
    * @returns:     {string} - $body에 추가
    ------------------------------------------------------------------*/
    KAuction.prototype.setUserOS = function() {
        var OSName = "";
        if (navigator.appVersion.indexOf("Win") != -1) OSName = "windows";
        if (navigator.appVersion.indexOf("Mac") != -1) OSName = "mac";
        if (navigator.appVersion.indexOf("X11") != -1) OSName = "unix";
        if (navigator.appVersion.indexOf("Linux") != -1) OSName = "linux";

        this.$body.addClass(OSName);
    }

    /*------------------------------------------------------------------ 
    * @function:    setUserAgent
    * @description: 사용자 디바이스 설정
    * @returns:     {string} - $body에 추가
    ------------------------------------------------------------------*/
    KAuction.prototype.setUserAgent = function() {
        if (navigator.userAgent.match(/Android|BlackBerry|iPhone|iPad|iPod|Opera Mini|IEMobile/i)) {
            this.$body.addClass('mobile');
        } else {
            this.$body.addClass('desktop');
            if (navigator.userAgent.match(/MSIE 9.0/)) {
                this.$body.addClass('ie9');
            }
        }
    }

    /*------------------------------------------------------------------
    * @function:    isVisibleXs
    * @description: 화면크기 확인 - XS(480px 이하)
    * @returns:     {$Element} - Body에 $('#ka-visible-xs') 추가
    ------------------------------------------------------------------*/
    KAuction.prototype.isVisibleXs = function() {
        (!$('#ka-visible-xs').length) && this.$body.append('<div id="ka-visible-xs" class="visible-xs" />');
        return $('#ka-visible-xs').is(':visible');
    }

    /*------------------------------------------------------------------
    * @function:    isVisibleSm
    * @description: 화면크기 확인 - SM(480px 이상)
    * @returns:     {$Element} - Body에 $('#ka-visible-sm') 추가
    ------------------------------------------------------------------*/
    KAuction.prototype.isVisibleSm = function() {
        (!$('#ka-visible-sm').length) && this.$body.append('<div id="ka-visible-sm" class="visible-sm" />');
        return $('#ka-visible-sm').is(':visible');
    }

    /*------------------------------------------------------------------
    * @function:    isVisibleMd
    * @description: 화면크기 확인 - MD(1024px 이상)
    * @returns:     {$Element} - Body에 $('#ka-visible-md') 추가
    ------------------------------------------------------------------*/
    KAuction.prototype.isVisibleMd = function() {
        (!$('#ka-visible-md').length) && this.$body.append('<div id="ka-visible-md" class="visible-md" />');
        return $('#ka-visible-md').is(':visible');
    }

    /*------------------------------------------------------------------
    * @function:    isVisibleLg
    * @description: 화면크기 확인 - LG(1200px 이상)
    * @returns:     {$Element} - Body에 $('#ka-visible-lg') 추가
    ------------------------------------------------------------------*/
    KAuction.prototype.isVisibleLg = function() {
        (!$('#ka-visible-lg').length) && this.$body.append('<div id="ka-visible-lg" class="visible-lg" />');
        return $('#ka-visible-lg').is(':visible');
    }

    /*------------------------------------------------------------------
    * @function:    getUserAgent
    * @description: 모바일 여부 확인
    * @returns:     {string} - mobile || desktop
    ------------------------------------------------------------------*/
    KAuction.prototype.getUserAgent = function() {
        return $('body').hasClass('mobile') ? "mobile" : "desktop";
    }

    /*------------------------------------------------------------------
    * @function:    setFullScreen
    * @description: 풀스크린
    ------------------------------------------------------------------*/
    KAuction.prototype.setFullScreen = function(element) {
        var requestMethod = element.requestFullScreen || element.webkitRequestFullScreen || element.mozRequestFullScreen || element.msRequestFullscreen;

        if (requestMethod) {
            requestMethod.call(element);
        } else if (typeof window.ActiveXObject !== "undefined") { // IE
            var wscript = new ActiveXObject("WScript.Shell");
            if (wscript !== null) {
                wscript.SendKeys("{F11}");
            }
        }
    }

    /*------------------------------------------------------------------
    * @function:    getColor
    * @description: CSS > RGBA
    * @param:       {string} 컬러 클래스 - primary, master...
    * @param:       {int} 투명도
    * @returns:     {rgba}
    ------------------------------------------------------------------*/
    KAuction.prototype.getColor = function(color, opacity) {
        opacity = parseFloat(opacity) || 1;

        var elem = $('.ka-colors').length ? $('.ka-colors') : $('<div class="ka-colors"></div>').appendTo('body');

        var colorElem = elem.find('[data-color="' + color + '"]').length ? elem.find('[data-color="' + color + '"]') : $('<div class="bg-' + color + '" data-color="' + color + '"></div>').appendTo(elem);

        var color = colorElem.css('background-color');

        var rgb = color.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
        var rgba = "rgba(" + rgb[1] + ", " + rgb[2] + ", " + rgb[3] + ', ' + opacity + ')';

        return rgba;
    }

    /*------------------------------------------------------------------
    * @function:    initSidebar
    * @description: 사이드바 오픈/클로즈
    * @param:       {(Element|JQuery)}
    * @requires:    ui/sidebar.js
    ------------------------------------------------------------------*/
    KAuction.prototype.initSidebar = function(context) {
        $('[data-kauction="sidebar"]', context).each(function() {
            var $sidebar = $(this)
            $sidebar.sidebar($sidebar.data())
        })
    }

    /*------------------------------------------------------------------
    * @function:    initDropDown
    * @description: 부트스트랩 드롭다운 메뉴 초기화
    * @param:       {(Element||JQuery)}
    * @requires:    bootstrap.js
    ------------------------------------------------------------------*/
    KAuction.prototype.initDropDown = function(context) {
        $('.dropdown-default', context).each(function() {
            var btn = $(this).find('.dropdown-menu').siblings('.dropdown-toggle');
            var offset = 0;
            var menuWidth = $(this).find('.dropdown-menu').actual('outerWidth');

            if (btn.actual('outerWidth') < menuWidth) {
                btn.width(menuWidth - offset);
                $(this).find('.dropdown-menu').width(btn.actual('outerWidth'));
            } else {
                $(this).find('.dropdown-menu').width(btn.actual('outerWidth'));
            }
        });
    }

    /*------------------------------------------------------------------
    * @function:    initFormGroupDefault
    * @description: 폼그룹 초기화
    * @param:       {(Element||JQuery)}
    ------------------------------------------------------------------*/
    KAuction.prototype.initFormGroupDefault = function (context) {
        $('.form-group.form-group-default', context).click(function () {
            $(this).find('input').focus();
        });

        if (!this.initFormGroupDefaultRun) {
            $('body').on('focus', '.form-group.form-group-default :input', function () {
                var type = $(this).attr("type");
                if (type == "checkbox" || type == "radio") {
                    return;
                }
                $('.form-group.form-group-default').removeClass('focused');
                $(this).parents('.form-group').addClass('focused');
            });

            $('body').on('blur', '.form-group.form-group-default :input', function () {
                var type = $(this).attr("type");
                if (type == "checkbox" || type == "radio") {
                    return;
                }
                $(this).parents('.form-group').removeClass('focused');
                if ($(this).val()) {
                    $(this).closest('.form-group').find('label').addClass('fade');
                } else {
                    $(this).closest('.form-group').find('label').removeClass('fade');
                }
            });

            this.initFormGroupDefaultRun = true;
        }
    }

    /*------------------------------------------------------------------
    * @function:    initSlidingTabs
    * @description: 부트스트랩 슬라이딩 탭 초기화
    * @param:       {(Element|JQuery)}
    * @requires:    bootstrap.js
    ------------------------------------------------------------------*/
    KAuction.prototype.initSlidingTabs = function(context) {
        $('a[data-toggle="tab"]', context).on('show.bs.tab', function(e) {
            e = $(e.target).parent().find('a[data-toggle=tab]');

            var hrefCurrent = e.data('target');
            if(hrefCurrent === undefined){
                hrefCurrent = e.attr('href');
            }

            if (!$(hrefCurrent).is('.slide-left, .slide-right')) return;
            $(hrefCurrent).addClass('sliding');

            setTimeout(function() {
                $(hrefCurrent).removeClass('sliding');
            }, 100);
        });
    }

   /*------------------------------------------------------------------
   * @function:    reponsiveTabs
   * @description: 부트스트랩 탭 반응형 핸들러
   ------------------------------------------------------------------*/
   KAuction.prototype.reponsiveTabs = function() {
        $('[data-init-reponsive-tabs="dropdownfx"]').each(function() {
        var drop = $(this);
        drop.addClass("d-none d-md-flex d-lg-flex d-xl-flex");
        var content = '<select class="cs-select cs-skin-slide full-width" data-init-plugin="cs-select">'
        for(var i = 1; i <= drop.children("li").length; i++){
            var li = drop.children("li:nth-child("+i+")");
            var selected ="";
            if(li.children('a').hasClass("active")){
                selected="selected";
            }
            var tabRef = li.children('a').attr('href');
            if(tabRef == "#" || ""){
                tabRef = li.children('a').attr('data-target')
            }
            content +='<option value="'+ tabRef+'" '+selected+'>';
            content += li.children('a').text();
            content += '</option>';
        }
        content +='</select>'
        drop.after(content);
        var select = drop.next()[0];
        $(select).on('change', function (e) {
            var optionSelected = $("option:selected", this);
            var valueSelected = this.value;
            var tabLink = drop.find('a[data-target="'+valueSelected+'"]');
            if(tabLink.length == 0){
                tabLink = drop.find('a[data-target="'+valueSelected+'"]')
            }
            tabLink.tab('show')
        })
        $(select).wrap('<div class="nav-tab-dropdown cs-wrapper full-width d-lg-none d-xl-none d-md-none"></div>');
        new SelectFx(select);
        });
    }

    /*------------------------------------------------------------------
    * @function:    initProgressBars
    * @description: 프로그레스바 초기화
    ------------------------------------------------------------------*/
    KAuction.prototype.initProgressBars = function() {
        $(window).on('load', function() {
            // FF SVG 백그라운드 애니메이션 해결
            $('.progress-bar-indeterminate, .progress-circle-indeterminate, .mapplic-pin').hide().show(0);
        });
    }

    /*------------------------------------------------------------------
    * @function:    initInputFile
    * @description: 부트스트랩 파일 인풋 초기화
    ------------------------------------------------------------------*/
    KAuction.prototype.initInputFile = function() {
        $(document).on('change', '.btn-file :file', function() {
            var input = $(this),
            numFiles = input.get(0).files ? input.get(0).files.length : 1,
            label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
            input.trigger('fileselect', [numFiles, label]);
        });

        $('.btn-file :file').on('fileselect', function(event, numFiles, label) {
            var input = $(this).parents('.input-group').find(':text'),
                log = numFiles > 1 ? numFiles + ' files selected' : label;
            if( input.length ) {
                input.val(log);
            } else {
                $(this).parent().html(log);
            }
        });
    }

    /*------------------------------------------------------------------
    * @function:    initTooltipPlugin
    * @description: 부트스트랩 툴팁 초기화
    * @param:       {(Element||JQuery)}
    * @requires:    bootstrap.js
    ------------------------------------------------------------------*/
    KAuction.prototype.initTooltipPlugin = function(context) {
        $.fn.tooltip && $('[data-toggle="tooltip"]', context).tooltip();
    }
    
    /*------------------------------------------------------------------
    * @function:    initSelect2Plugin
    * @description: select2 플러그인 드롭다운 초기화
    * @param:       {(Element||JQuery)}
    * @requires:    select2.js v4
    ------------------------------------------------------------------*/
    KAuction.prototype.initSelect2Plugin = function(context) {
        $.fn.select2 && $('[data-init-plugin="select2"]', context).each(function() {
            $(this).select2({
                minimumResultsForSearch: ($(this).attr('data-disable-search') == 'true' ? -1 : 1)
            }).on('select2:open', function() {
                $.fn.scrollbar && $('.select2-results__options').scrollbar({
                    ignoreMobile: false
                })
            });
        });
    }

    /*------------------------------------------------------------------
    * @function:    initScrollBarPlugin
    * @description: 전역 스크롤 플러그인 초기화
    * @param:       {(Element||JQuery)}
    * @requires:    jquery-scrollbar.js
    ------------------------------------------------------------------*/
    KAuction.prototype.initScrollBarPlugin = function(context) {
        $.fn.scrollbar && $('.scrollable', context).scrollbar({
            ignoreOverlay: false
        });
    }

    /*------------------------------------------------------------------
    * @function:    initListView
    * @description: iOS 리스트뷰 플러그인 초기화
    * @param:       {(Element||JQuery)}
    * @example:     <caption>data-init-list-view="ioslist"</caption>
    * @requires:    jquery-ioslist.js
    ------------------------------------------------------------------*/
    KAuction.prototype.initListView = function(context) {
        $.fn.ioslist && $('[data-init-list-view="ioslist"]', context).ioslist();
        $.fn.scrollbar && $('.list-view-wrapper', context).scrollbar({
            ignoreOverlay: false
        });
    }

    /*------------------------------------------------------------------
    * @function:    initSelectFxPlugin
    * @description: selectFX 플러그인 초기화
    * @param:       {(Element||JQuery)}
    * @example:     <caption>select[data-init-plugin="cs-select"]</caption>
    ------------------------------------------------------------------*/
    KAuction.prototype.initSelectFxPlugin = function(context) {
        window.SelectFx && $('select[data-init-plugin="cs-select"]', context).each(function() {
            var el = $(this).get(0);
            $(el).wrap('<div class="cs-wrapper"></div>');
            new SelectFx(el);
        });
    }

    /*------------------------------------------------------------------
    * @function:    initUnveilPlugin
    * @description: Unveil 플러그인 초기화
    * @param:       {(Element||JQuery)}
    * @requires:    jquery.unveil.js
    ------------------------------------------------------------------*/
    KAuction.prototype.initUnveilPlugin = function(context) {
        $.fn.unveil && $("img", context).unveil();
    }

    /*------------------------------------------------------------------
    * @function:    initValidatorPlugin
    * @description: jquery-validate 메소드 오버라이드 및 초기화
    * @requires:    jquery-validate.js
    ------------------------------------------------------------------*/
    KAuction.prototype.initValidatorPlugin = function() {
        $.validator && $.validator.setDefaults({
            ignore: "", // 히든필드검증(cs-select)
            showErrors: function(errorMap, errorList) {
                var $this = this;
                $.each(this.successList, function(index, value) {
                    var parent = $(this).closest('.form-group-attached');
                    if (parent.length) return $(value).popover("hide");
                });
                return $.each(errorList, function(index, value) {
                    var parent = $(value.element).closest('.form-group-attached');
                    if (!parent.length) {
                        return $this.defaultShowErrors();
                    }
                    var _popover;
                    _popover = $(value.element).popover({
                        trigger: "manual",
                        placement: "top",
                        html: true,
                        container: parent.closest('form'),
                        content: value.message
                    });
                    var parent = $(value.element).closest('.form-group');
                    parent.addClass('has-error');
                    $(value.element).popover("show");
                });
            },
            onfocusout: function(element) {
                var parent = $(element).closest('.form-group');
                if ($(element).valid()) {
                    parent.removeClass('has-error');
                    parent.next('.error').remove();
                }
            },
            onkeyup: function(element) {
                var parent = $(element).closest('.form-group');
                if ($(element).valid()) {
                    $(element).removeClass('error');
                    parent.removeClass('has-error');
                    parent.next('label.error').remove();
                    parent.find('label.error').remove();
                } else {
                    parent.addClass('has-error');
                }
            },
            errorPlacement: function(error, element) {
                var parent = $(element).closest('.form-group');
                if (parent.hasClass('form-group-default')) {
                    parent.addClass('has-error');
                    error.insertAfter(parent);
                } else if(element.parent().hasClass('checkbox')) {
                    error.insertAfter(element.parent());
                } else {
                    error.insertAfter(element);
                }
            }
        });
    }

    /*------------------------------------------------------------------
    * @function:    setBackgroundImage
    * @description: div 이미지 로드 / 데이터 API 
    ------------------------------------------------------------------*/
    KAuction.prototype.setBackgroundImage = function() {
        $('[data-kauction-bg-image]').each(function() {
            var _elem = $(this)
            var defaults = {
                bgImage: "",
                lazyLoad: 'true',
                progressType: '',
                progressColor:'',
                bgOverlay:'',
                bgOverlayClass:'',
                overlayOpacity:0,
            }
            var data = _elem.data();
            $.extend( defaults, data );
            var url = defaults.bgImage;
            var color = defaults.bgOverlay;
            var opacity = defaults.overlayOpacity;

            var overlay = $('<div class="bg-overlay"></div>');
            overlay.addClass(defaults.bgOverlayClass);
            overlay.css({
                'background-color': color,
                'opacity': 1
            });
            _elem.append(overlay);

            var img = new Image();
            img.src = url;
            img.onload = function(){
                _elem.css({
                    'background-image': 'url(' + url + ')'
                });
                _elem.children('.bg-overlay').css({'opacity': opacity});
            }

        })
    }

    /*------------------------------------------------------------------
    * @function:    init
    * @description: 코어 컴포넌트 초기화
    ------------------------------------------------------------------*/
    KAuction.prototype.init = function() {
        this.initSidebar();
        this.setBackgroundImage();
        this.initDropDown();
        this.initFormGroupDefault();
        this.initSlidingTabs();
        this.initProgressBars();
        this.initTooltipPlugin();
        this.initSelect2Plugin();
        this.initScrollBarPlugin();
        this.initSelectFxPlugin();
        this.initUnveilPlugin();
        this.initValidatorPlugin();
        this.initListView();
        this.initInputFile();
        this.reponsiveTabs();
    }

    $.KAuction = new KAuction();
    $.KAuction.Constructor = KAuction;

})(window.jQuery);


/*------------------------------------------------------------------
 * 플러그인 오버라이드 / selectFx.js v1.0.0
 ------------------------------------------------------------------*/
(function(window) {
    'use strict';

    /* jquery.nicescroll.js */
    function hasParent(e, p) {
        if (!e) return false;
        var el = e.target || e.srcElement || e || false;
        while (el && el != p) {
            el = el.parentNode || false;
        }
        return (el !== false);
    };

    function extend(a, b) {
        for (var key in b) {
            if (b.hasOwnProperty(key)) {
                a[key] = b[key];
            }
        }
        return a;
    }

    /* SelectFx */
    function SelectFx(el, options) {
        this.el = el;
        this.options = extend({}, this.options);
        extend(this.options, options);
        this._init();
    }

    /* JS = jQuery closest() */
    function closest(elem, selector) {
        var matchesSelector = elem.matches || elem.webkitMatchesSelector || elem.mozMatchesSelector || elem.msMatchesSelector;
        while (elem) {
            if (matchesSelector.bind(elem)(selector)) {
                return elem;
            } else {
                elem = elem.parentElement;
            }
        }
        return false;
    }

    /* JS = jQuery offset() */
    function offset(el) {
        return {
            left: el.getBoundingClientRect().left + window.pageXOffset - el.ownerDocument.documentElement.clientLeft,
            top: el.getBoundingClientRect().top + window.pageYOffset - el.ownerDocument.documentElement.clientTop
        }
    }

    /* JS = jQuery after() */
    function insertAfter(newNode, referenceNode) {
            referenceNode.parentNode.insertBefore(newNode, referenceNode.nextSibling);
        }
    
    /* SelectFx 옵션 */
    SelectFx.prototype.options = {
        newTab: true,
        stickyPlaceholder: true,
        container: 'body',
        onChange: function(el) {
            var event = document.createEvent('HTMLEvents');
            event.initEvent('change', true, false);
            el.dispatchEvent(event);
        }
    }

    /* 초기화 */
    SelectFx.prototype._init = function() {
        var selectedOpt = document.querySelector('option[selected]');
        this.hasDefaultPlaceholder = selectedOpt && selectedOpt.disabled;
        this.selectedOpt = selectedOpt || this.el.querySelector('option');
        this._createSelectEl();
        this.selOpts = [].slice.call(this.selEl.querySelectorAll('li[data-option]'));
        this.selOptsCount = this.selOpts.length;
        this.current = this.selOpts.indexOf(this.selEl.querySelector('li.cs-selected')) || -1;
        this.selPlaceholder = this.selEl.querySelector('span.cs-placeholder');
        this._initEvents();
        this.el.onchange = function() {
            var index = this.selectedIndex;
            var inputText = this.children[index].innerHTML.trim();
        }
    }

    /* 셀렉트 엘리먼트 */
    SelectFx.prototype._createSelectEl = function() {
        var self = this,
            options = '',
            createOptionHTML = function(el) {
                var optclass = '',
                    classes = '',
                    link = '';

                if (el.selectedOpt && !this.foundSelected && !this.hasDefaultPlaceholder) {
                    classes += 'cs-selected ';
                    this.foundSelected = true;
                }
                if (el.getAttribute('data-class')) {
                    classes += el.getAttribute('data-class');
                }
                if (el.getAttribute('data-link')) {
                    link = 'data-link=' + el.getAttribute('data-link');
                }
                if (classes !== '') {
                    optclass = 'class="' + classes + '" ';
                }
                return '<li ' + optclass + link + ' data-option data-value="' + el.value + '"><span>' + el.textContent + '</span></li>';
            };

        [].slice.call(this.el.children).forEach(function(el) {
            if (el.disabled) {
                return;
            }

            var tag = el.tagName.toLowerCase();

            if (tag === 'option') {
                options += createOptionHTML(el);
            } else if (tag === 'optgroup') {
                options += '<li class="cs-optgroup"><span>' + el.label + '</span><ul>';
                [].slice.call(el.children).forEach(function(opt) {
                    options += createOptionHTML(opt);
                })
                options += '</ul></li>';
            }
        });

        var opts_el = '<div class="cs-options"><ul>' + options + '</ul></div>';
        this.selEl = document.createElement('div');
        this.selEl.className = this.el.className;
        this.selEl.tabIndex = this.el.tabIndex;
        this.selEl.innerHTML = '<span class="cs-placeholder">' + this.selectedOpt.textContent + '</span>' + opts_el;
        this.el.parentNode.appendChild(this.selEl);
        this.selEl.appendChild(this.el);

        var backdrop = document.createElement('div');
        backdrop.className = 'cs-backdrop';
        this.selEl.appendChild(backdrop);
    }

    /* 이벤트 초기화 */
    SelectFx.prototype._initEvents = function() {
        var self = this;

        this.selPlaceholder.addEventListener('click', function() {
            self._toggleSelect();
        });

        this.selOpts.forEach(function(opt, idx) {
            opt.addEventListener('click', function() {
                self.current = idx;
                self._changeOption();
                // close select elem
                self._toggleSelect();
            });
        });

        document.addEventListener('click', function(ev) {
            var target = ev.target;
            if (self._isOpen() && target !== self.selEl && !hasParent(target, self.selEl)) {
                self._toggleSelect();
            }
        });

        this.selEl.addEventListener('keydown', function(ev) {
            var keyCode = ev.keyCode || ev.which;

            switch (keyCode) {
                // 업
                case 38:
                    ev.preventDefault();
                    self._navigateOpts('prev');
                    break;
                    // 다운
                case 40:
                    ev.preventDefault();
                    self._navigateOpts('next');
                    break;
                    // 스페이스
                case 32:
                    ev.preventDefault();
                    if (self._isOpen() && typeof self.preSelCurrent != 'undefined' && self.preSelCurrent !== -1) {
                        self._changeOption();
                    }
                    self._toggleSelect();
                    break;
                    // 엔터
                case 13:
                    ev.preventDefault();
                    if (self._isOpen() && typeof self.preSelCurrent != 'undefined' && self.preSelCurrent !== -1) {
                        self._changeOption();
                        self._toggleSelect();
                    }
                    break;
                    // 이스케이프
                case 27:
                    ev.preventDefault();
                    if (self._isOpen()) {
                        self._toggleSelect();
                    }
                    break;
            }
        });
    }

    /* 업/다운키 이동 */
    SelectFx.prototype._navigateOpts = function(dir) {
        if (!this._isOpen()) {
            this._toggleSelect();
        }

        var tmpcurrent = typeof this.preSelCurrent != 'undefined' && this.preSelCurrent !== -1 ? this.preSelCurrent : this.current;

        if (dir === 'prev' && tmpcurrent > 0 || dir === 'next' && tmpcurrent < this.selOptsCount - 1) {
            this.preSelCurrent = dir === 'next' ? tmpcurrent + 1 : tmpcurrent - 1;
            this._removeFocus();
            classie.add(this.selOpts[this.preSelCurrent], 'cs-focus');
        }
    }

    /* 토글 */
    SelectFx.prototype._toggleSelect = function() {
        var backdrop = this.selEl.querySelector('.cs-backdrop');
        var container = document.querySelector(this.options.container);
        var mask = container.querySelector('.dropdown-mask');
        var csOptions = this.selEl.querySelector('.cs-options');
        var csPlaceholder = this.selEl.querySelector('.cs-placeholder');
        var csPlaceholderWidth = csPlaceholder.offsetWidth;
        var csPlaceholderHeight = csPlaceholder.offsetHeight;
        var csOptionsWidth = csOptions.scrollWidth;

        if (this._isOpen()) {
            if (this.current !== -1) {
                this.selPlaceholder.textContent = this.selOpts[this.current].textContent;
            }

            var dummy = this.selEl.data;
            var parent = dummy.parentNode;
            insertAfter(this.selEl, dummy);
            this.selEl.removeAttribute('style');
            parent.removeChild(dummy);
            var x = this.selEl.clientHeight;

            backdrop.style.transform = backdrop.style.webkitTransform = backdrop.style.MozTransform = backdrop.style.msTransform = backdrop.style.OTransform = 'scale3d(1,1,1)';
            classie.remove(this.selEl, 'cs-active');
            mask.style.display = 'none';
            csOptions.style.overflowY = 'hidden';
            csOptions.style.width = 'auto';

            var parentFormGroup = closest(this.selEl, '.form-group');
            parentFormGroup && classie.removeClass(parentFormGroup, 'focused');

        } else {
            if (this.hasDefaultPlaceholder && this.options.stickyPlaceholder) {
                this.selPlaceholder.textContent = this.selectedOpt.textContent;
            }

            var dummy;
            if (this.selEl.parentNode.querySelector('.dropdown-placeholder')) {
                dummy = this.selEl.parentNode.querySelector('.dropdown-placeholder');
            } else {
                dummy = document.createElement('div');
                classie.add(dummy, 'dropdown-placeholder');
                insertAfter(dummy, this.selEl);
            }

            dummy.style.height = csPlaceholderHeight + 'px';
            dummy.style.width = this.selEl.offsetWidth + 'px';

            this.selEl.data = dummy;
            this.selEl.style.position = 'absolute';
            var offsetselEl = offset(this.selEl);

            this.selEl.style.left = offsetselEl.left + 'px';
            this.selEl.style.top = offsetselEl.top + 'px';

            container.appendChild(this.selEl);

            var contentHeight = csOptions.offsetHeight;
            var originalHeight = csPlaceholder.offsetHeight;
            var contentWidth = csOptions.offsetWidth;
            var originalWidth = csPlaceholder.offsetWidth;
            var scaleV = contentHeight / originalHeight;
            var scaleH = (contentWidth > originalWidth) ? contentWidth / originalWidth : 1.05;
            backdrop.style.transform = backdrop.style.webkitTransform = backdrop.style.MozTransform = backdrop.style.msTransform = backdrop.style.OTransform = 'scale3d(' + 1 + ', ' + scaleV + ', 1)';

            if (!mask) {
                mask = document.createElement('div');
                classie.add(mask, 'dropdown-mask');
                container.appendChild(mask);
            }

            mask.style.display = 'block';

            classie.add(this.selEl, 'cs-active');

            var resizedWidth = (csPlaceholderWidth < csOptionsWidth) ? csOptionsWidth : csPlaceholderWidth;

            this.selEl.style.width = resizedWidth + 'px';
            this.selEl.style.height = originalHeight + 'px';
            csOptions.style.width = '100%';

            setTimeout(function() {
                csOptions.style.overflowY = 'auto';
            }, 300);

        }
    }

    /* 옵션 변경 */
    SelectFx.prototype._changeOption = function() {
        if (typeof this.preSelCurrent != 'undefined' && this.preSelCurrent !== -1) {
            this.current = this.preSelCurrent;
            this.preSelCurrent = -1;
        }
        
        // 현재 옵션
        var opt = this.selOpts[this.current];

        // 현재 선택값 변경
        this.selPlaceholder.textContent = opt.textContent;

        // 네이티브 선택값 변경
        this.el.value = opt.getAttribute('data-value');

        // 클래스 처리
        var oldOpt = this.selEl.querySelector('li.cs-selected');
        if (oldOpt) {
            classie.remove(oldOpt, 'cs-selected');
        }
        classie.add(opt, 'cs-selected');

        if (opt.getAttribute('data-link')) {
            // open in new tab?
            if (this.options.newTab) {
                window.open(opt.getAttribute('data-link'), '_blank');
            } else {
                window.location = opt.getAttribute('data-link');
            }
        }

        // 콜백
        this.options.onChange(this.el);
    }

    SelectFx.prototype._isOpen = function(opt) {
        return classie.has(this.selEl, 'cs-active');
    }

    SelectFx.prototype._removeFocus = function(opt) {
        var focusEl = this.selEl.querySelector('li.cs-focus')
        if (focusEl) {
            classie.remove(focusEl, 'cs-focus');
        }
    }

    window.SelectFx = SelectFx;

})(window);

/*------------------------------------------------------------------
 * 프로그레스바 / 원형
 ------------------------------------------------------------------*/
(function($) {
    'use strict';

    /* 클래스 정의 */
    var Progress = function(element, options) {
        this.$element = $(element);
        this.options = $.extend(true, {}, $.fn.circularProgress.defaults, options);

        // DOM 추가
        this.$container = $('<div class="progress-circle"></div>');

        this.$element.attr('data-color') && this.$container.addClass('progress-circle-' + this.$element.attr('data-color'));
        this.$element.attr('data-thick') && this.$container.addClass('progress-circle-thick');

        this.$pie = $('<div class="pie"></div>');
        this.$pie.$left = $('<div class="left-side half-circle"></div>');
        this.$pie.$right = $('<div class="right-side half-circle"></div>');
        this.$pie.append(this.$pie.$left).append(this.$pie.$right);
        this.$container.append(this.$pie).append('<div class="shadow"></div>');
        this.$element.after(this.$container);

        this.val = this.$element.val();
        var deg = perc2deg(this.val);

        if (this.val <= 50) {
            this.$pie.$right.css('transform', 'rotate(' + deg + 'deg)');
        } else {
            this.$pie.css('clip', 'rect(auto, auto, auto, auto)');
            this.$pie.$right.css('transform', 'rotate(180deg)');
            this.$pie.$left.css('transform', 'rotate(' + deg + 'deg)');
        }
    }

    Progress.prototype.value = function(val) {
        if (typeof val == 'undefined') return;

        var deg = perc2deg(val);

        this.$pie.removeAttr('style');
        this.$pie.$right.removeAttr('style');
        this.$pie.$left.removeAttr('style');

        if (val <= 50) {
            this.$pie.$right.css('transform', 'rotate(' + deg + 'deg)');
        } else {
            this.$pie.css('clip', 'rect(auto, auto, auto, auto)');
            this.$pie.$right.css('transform', 'rotate(180deg)');
            this.$pie.$left.css('transform', 'rotate(' + deg + 'deg)');
        }
    }

    /* 플러그인 정의 */
    function Plugin(option) {
        return this.filter(':input').each(function() {
            var $this = $(this);
            var data = $this.data('ka.circularProgress');
            var options = typeof option == 'object' && option;

            if (!data) $this.data('ka.circularProgress', (data = new Progress(this, options)));
            if (typeof option == 'string') data[option]();
            else if (options.hasOwnProperty('value')) data.value(options.value);
        })
    }

    var old = $.fn.circularProgress

    $.fn.circularProgress = Plugin
    $.fn.circularProgress.Constructor = Progress
    $.fn.circularProgress.defaults = {
        value: 0
    }

    /* NO CONFLICT */
    $.fn.circularProgress.noConflict = function() {
        $.fn.circularProgress = old;
        return this;
    }

    /* DATA API */
    $(window).on('load', function() {
        $('[data-kauction-progress="circle"]').each(function() {
            var $progress = $(this)
            $progress.circularProgress($progress.data())
        })
    })

    function perc2deg(p) {
        return parseInt(p / 100 * 360);
    }

})(window.jQuery);

/*------------------------------------------------------------------
 * Cards
 ------------------------------------------------------------------*/
(function($) {
    'use strict';

    /* 카드 클래스 정의 */
    var Card = function(element, options) {
        this.$element = $(element);
        this.options = $.extend(true, {}, $.fn.card.defaults, options);
        this.$loader = null;
        this.$body = this.$element.find('.card-body');
    }

    /* 버튼 액션 */
    Card.prototype.collapse = function() {
        var icon = this.$element.find(this.options.collapseButton + ' > i');
        var heading = this.$element.find('.card-header');

        this.$body.stop().slideToggle("fast");

        if (this.$element.hasClass('card-collapsed')) {
            this.$element.removeClass('card-collapsed');
            $.isFunction(this.options.onExpand) && this.options.onExpand(this);
            return
        }
        this.$element.addClass('card-collapsed');
        $.isFunction(this.options.onCollapse) && this.options.onCollapse(this);
    }

    Card.prototype.close = function() {
        this.$element.remove();
        $.isFunction(this.options.onClose) && this.options.onClose(this);
    }

    Card.prototype.maximize = function() {
        var icon = this.$element.find(this.options.maximizeButton + ' > i');

        if (this.$element.hasClass('card-maximized')) {
            this.$element.removeClass('card-maximized');
            this.$element.attr('style','');
            $.isFunction(this.options.onRestore) && this.options.onRestore(this);
        } else {
            var contentEl = $('.kauction-content-wrapper > .content');
            var header = $('.header');
            var left = 0;
            if(contentEl){
                left = contentEl[0].getBoundingClientRect().left;
                var style = window.getComputedStyle(contentEl[0]);
                left = left + (parseFloat(style["marginLeft"])+parseFloat(style["paddingLeft"]));
            }
            var headerHeight = header.height();

            this.$element.addClass('card-maximized');
            this.$element.css('left', left);
            this.$element.css('top', headerHeight);

            $.isFunction(this.options.onMaximize) && this.options.onMaximize(this);
        }
    }

    /* 옵션 */
    Card.prototype.refresh = function(refresh) {
        var toggle = this.$element.find(this.options.refreshButton);

        if (refresh) {
            if (this.$loader && this.$loader.is(':visible')) return;
            if (!$.isFunction(this.options.onRefresh)) return;
            this.$loader = $('<div class="card-progress"></div>');
            this.$loader.css({
                'background-color': 'rgba(' + this.options.overlayColor + ',' + this.options.overlayOpacity + ')'
            });

            var elem = '';
            if (this.options.progress == 'circle') {
                elem += '<div class="progress-circle-indeterminate progress-circle-' + this.options.progressColor + '"></div>';
            } else if (this.options.progress == 'bar') {

                elem += '<div class="progress progress-small">';
                elem += '    <div class="progress-bar-indeterminate progress-bar-' + this.options.progressColor + '"></div>';
                elem += '</div>';
            } else if (this.options.progress == 'circle-lg') {
                toggle.addClass('refreshing');
                var iconOld = toggle.find('> i').first();
                var iconNew;
                if (!toggle.find('[class$="-animated"]').length) {
                    iconNew = $('<i/>');
                    iconNew.css({
                        'position': 'absolute',
                        'top': iconOld.position().top,
                        'left': iconOld.position().left
                    });
                    iconNew.addClass('card-icon-refresh-lg-' + this.options.progressColor + '-animated');
                    toggle.append(iconNew);
                } else {
                    iconNew = toggle.find('[class$="-animated"]');
                }

                iconOld.addClass('fade');
                iconNew.addClass('active');

            } else {
                elem += '<div class="progress progress-small">';
                elem += '    <div class="progress-bar-indeterminate progress-bar-' + this.options.progressColor + '"></div>';
                elem += '</div>';
            }

            this.$loader.append(elem);
            this.$element.append(this.$loader);

            // FF 호환
            var _loader = this.$loader;
            setTimeout(function() {
                this.$loader.remove();
                this.$element.append(_loader);
            }.bind(this), 300);

            this.$loader.fadeIn();

            $.isFunction(this.options.onRefresh) && this.options.onRefresh(this);

        } else {
            var _this = this;
            this.$loader.fadeOut(function() {
                $(this).remove();
                if (_this.options.progress == 'circle-lg') {
                    var iconNew = toggle.find('.active');
                    var iconOld = toggle.find('.fade');
                    iconNew.removeClass('active');
                    iconOld.removeClass('fade');
                    toggle.removeClass('refreshing');

                }
                _this.options.refresh = false;
            });
        }
    }

    Card.prototype.error = function(error) {
        if (error) {
            var _this = this;
            this.$element.kaNotification({
                style: 'bar',
                message: error,
                position: 'top',
                timeout: 0,
                type: 'danger',
                onShown: function() {
                    _this.$loader.find('> div').fadeOut()
                },
                onClosed: function() {
                    _this.refresh(false)
                }
            }).show();
        }
    }

    /* 카드 플러그인 정의 */
    function Plugin(option) {
        return this.each(function() {
            var $this = $(this);
            var data = $this.data('ka.card');
            var options = typeof option == 'object' && option;

            if (!data) $this.data('ka.card', (data = new Card(this, options)));
            if (typeof option == 'string') data[option]();
            else if (options.hasOwnProperty('refresh')) data.refresh(options.refresh);
            else if (options.hasOwnProperty('error')) data.error(options.error);
        })
    }

    var old = $.fn.card

    $.fn.card = Plugin
    $.fn.card.Constructor = Card

    $.fn.card.defaults = {
        progress: 'circle',
        progressColor: 'master',
        refresh: false,
        error: null,
        overlayColor: '255,255,255',
        overlayOpacity: 0.8,
        refreshButton: '[data-toggle="refresh"]',
        maximizeButton: '[data-toggle="maximize"]',
        collapseButton: '[data-toggle="collapse"]',
        closeButton: '[data-toggle="close"]'

        // onRefresh: function(portlet) {},
        // onCollapse: function(portlet) {},
        // onExpand: function(portlet) {},
        // onMaximize: function(portlet) {},
        // onRestore: function(portlet) {},
        // onClose: function(portlet) {}
    }

    /* NO CONFLICT */
    $.fn.card.noConflict = function() {
        $.fn.card = old;
        return this;
    }

    /* DATA API */
    $(document).on('click.ka.card.data-api', '[data-toggle="collapse"]', function(e) {
        var $this = $(this);
        var $target = $this.closest('.card');
        if ($this.is('a')) e.preventDefault();
        $target.data('ka.card') && $target.card('collapse');
    })

    $(document).on('click.ka.card.data-api', '[data-toggle="close"]', function(e) {
        var $this = $(this);
        var $target = $this.closest('.card');
        if ($this.is('a')) e.preventDefault();
        $target.data('ka.card') && $target.card('close');
    })

    $(document).on('click.ka.card.data-api', '[data-toggle="refresh"]', function(e) {
        var $this = $(this);
        var $target = $this.closest('.card');
        if ($this.is('a')) e.preventDefault();
        $target.data('ka.card') && $target.card({
            refresh: true
        })
    })

    $(document).on('click.ka.card.data-api', '[data-toggle="maximize"]', function(e) {
        var $this = $(this);
        var $target = $this.closest('.card');
        if ($this.is('a')) e.preventDefault();
        $target.data('ka.card') && $target.card('maximize');
    })

    $(window).on('load', function() {
        $('[data-kauction="card"]').each(function() {
            var $card = $(this)
            $card.card($card.data())
        })
    })

})(window.jQuery);

/*------------------------------------------------------------------
 * 모바일 뷰
 ------------------------------------------------------------------*/
(function($) {
    'use strict';

    var MobileView = function(element, options) {
        var self = this;
        self.options = $.extend(true, {}, $.fn.kaMobileViews.defaults, options);
        self.element = $(element);
        self.element.on('click', function(e) {
            e.preventDefault();
            var data = self.element.data();
            var el = $(data.viewPort);
            var toView = data.toggleView;
            if (data.toggleView != null) {
                el.children().last().children('.view').hide();
                $(data.toggleView).show();
            }
            else{
                 toView = el.last();
            }
            el.toggleClass(data.viewAnimation);
            self.options.onNavigate(toView, data.viewAnimation);
            return false;
        })
        return this;
    };
    $.fn.kaMobileViews = function(options) {
        return new MobileView(this, options);
    };

    $.fn.kaMobileViews.defaults = {
        onNavigate: function(view, animation) {}
    }

    $(window).on('load', function() {
        $('[data-navigate="view"]').each(function() {
            var $mobileView = $(this)
            $mobileView.kaMobileViews();
        })
    });
})(window.jQuery);

/*------------------------------------------------------------------
 * 패럴랙스 플러그인
 ------------------------------------------------------------------*/
(function($) {
    'use strict';
    
    /* 패럴랙스 클래스 정의 */
    var Parallax = function(element, options) {
        this.$element = $(element);
        this.options = $.extend(true, {}, $.fn.parallax.defaults, options);
        this.$coverPhoto = this.$element.find('.cover-photo');
        this.$content = this.$element.find('.inner');

        if (this.$coverPhoto.find('> img').length) {
            var img = this.$coverPhoto.find('> img');
            this.$coverPhoto.css('background-image', 'url(' + img.attr('src') + ')');
            img.remove();
        }

        if(this.options.scrollElement !== window) {
            $(this.options.scrollElement).on('scroll', function() {
                $(element).parallax('animate');
            });
        }
    }

    Parallax.prototype.animate = function() {

        var scrollPos;
        var coverWidth = this.$element.height();
        var opacityKeyFrame = coverWidth * 50 / 100;
        var direction = 'translateX';

        scrollPos = $(this.options.scrollElement).scrollTop();
        direction = 'translateY';

        this.$coverPhoto.css({
            'transform': direction + '(' + scrollPos * this.options.speed.coverPhoto + 'px)'
        });

        this.$content.css({
            'transform': direction + '(' + scrollPos * this.options.speed.content + 'px)',
        });

        if (scrollPos > opacityKeyFrame) {
            this.$content.css({
                'opacity': 1 - scrollPos / 1200
            });
        } else {
            this.$content.css({
                'opacity': 1
            });
        }
    }

    /* 패럴랙스 플러그인 정의 */
    function Plugin(option) {
        return this.each(function() {
            var $this = $(this);
            var data = $this.data('ka.parallax');
            var options = $.extend(true, {}, typeof option == 'object' ? option : {}, $this.data());
            
            if (!data) $this.data('ka.parallax', (data = new Parallax(this, options)));
            if (typeof option == 'string') data[option]();
        })
    }

    var old = $.fn.parallax

    $.fn.parallax = Plugin
    $.fn.parallax.Constructor = Parallax
    $.fn.parallax.defaults = {
        speed: {
            coverPhoto: 0.3,
            content: 0.17
        },
        scrollElement: window
    }

    /* NO CONFLICT */
    $.fn.parallax.noConflict = function() {
        $.fn.parallax = old;
        return this;
    }

    /* DATA API */
    $(window).on('load', function() {
        $('[data-kauction="parallax"]').each(function() {
            var $parallax = $(this)
            $parallax.parallax($parallax.data())
        })
    });

    $(window).on('scroll', function() {
        if (Modernizr.touch) {
            return;
        }
        $('[data-kauction="parallax"]').parallax('animate');
    });

})(window.jQuery);

/*------------------------------------------------------------------
 * 사이드바
 *------------------------------------------------------------------*/
 (function($) {
    'use strict';

    var Sidebar = function(element, options) {
         this.$element = $(element);
         this.$body = $('body');
         this.options = $.extend(true, {}, $.fn.sidebar.defaults, options);
         this.bezierEasing = [.05, .74, .27, .99];
         this.cssAnimation = true;
         this.css3d = true;
         this.sideBarWidth = 280;
         this.sideBarWidthCondensed = 280 - 70;
         this.$sidebarMenu = this.$element.find('.sidebar-menu > ul');
         this.$kaContainer = $(this.options.kaContainer);

         if (!this.$sidebarMenu.length) return;

         ($.KAuction.getUserAgent() == 'desktop') && this.$sidebarMenu.scrollbar({
             ignoreOverlay: false,
             disableBodyScroll :(this.$element.data("disableBodyScroll") == true)? true : false
         });

         if (!Modernizr.csstransitions)
             this.cssAnimation = false;
         if (!Modernizr.csstransforms3d)
             this.css3d = false;

         (typeof angular === 'undefined') && $(document).on('click', '.sidebar-menu a', function(e) {

             if ($(this).parent().children('.sub-menu') === false) {
                 return;
             }
             var el = $(this);
             var parent = $(this).parent().parent();
             var li = $(this).parent();
             var sub = $(this).parent().children('.sub-menu');

             if(li.hasClass("open active")){
                el.children('.arrow').removeClass("open active");
                sub.slideUp(200, function() {
                    li.removeClass("open active");
                });

             }else{
                parent.children('li.open').children('.sub-menu').slideUp(200);
                parent.children('li.open').children('a').children('.arrow').removeClass('open active');
                parent.children('li.open').removeClass("open active");
                el.children('.arrow').addClass("open active");
                sub.slideDown(200, function() {
                    li.addClass("open active");
                });
             }
             //e.preventDefault();
         });

         // 토글 사이드바 슬라이드
         $('.sidebar-slide-toggle').on('click touchend', function(e) {
             e.preventDefault();
             $(this).toggleClass('active');
             var el = $(this).attr('data-kauction-toggle');
             if (el != null) {
                 $(el).toggleClass('show');
             }
         });

         var _this = this;

         function sidebarMouseEnter(e) {
             var _sideBarWidthCondensed = _this.$body.hasClass("rtl") ? -_this.sideBarWidthCondensed : _this.sideBarWidthCondensed;
             var menuOpenCSS = (this.css3d == true ? 'translate3d(' + _sideBarWidthCondensed + 'px, 0,0)' : 'translate(' + _sideBarWidthCondensed + 'px, 0)');

             if ($.KAuction.isVisibleSm() || $.KAuction.isVisibleXs()) {
                 return false
             }
             if ($('.close-sidebar').data('clicked')) {
                 return;
             }
             if (_this.$body.hasClass('menu-pin'))
                 return;

             _this.$element.css({
                 'transform': menuOpenCSS
             });
             _this.$body.addClass('sidebar-visible');
         }

         function sidebarMouseLeave(e) {
             var menuClosedCSS = (_this.css3d == true ? 'translate3d(0, 0,0)' : 'translate(0, 0)');

             if ($.KAuction.isVisibleSm() || $.KAuction.isVisibleXs()) {
                 return false
             }
             if (typeof e != 'undefined') {
                 var target = $(e.target);
                 if (target.parent('.kauction-sidebar').length) {
                     return;
                 }
             }
             if (_this.$body.hasClass('menu-pin'))
                 return;

             if ($('.sidebar-overlay-slide').hasClass('show')) {
                 $('.sidebar-overlay-slide').removeClass('show')
                 $("[data-kauction-toggle']").removeClass('active')
             }

             _this.$element.css({
                 'transform': menuClosedCSS
             });
             _this.$body.removeClass('sidebar-visible');
         }

         this.$element.bind('mouseenter mouseleave', sidebarMouseEnter);
         this.$kaContainer.bind('mouseover', sidebarMouseLeave);

         function toggleMenuPin(){
           var width = $(window).width();
           if(width < 1200){
             if($('body').hasClass('menu-pin')){
               $('body').removeClass('menu-pin')
               $('body').addClass('menu-unpinned')
             }
           } else {
             if($('body').hasClass('menu-unpinned')){
               $('body').addClass('menu-pin')
             }
           }
         }
         toggleMenuPin();
         $(document).bind('ready', toggleMenuPin);
         $(window).bind('resize', toggleMenuPin);
     }

     // 모바일뷰
     Sidebar.prototype.toggleSidebar = function(toggle) {
         var timer;
         var bodyColor = $('body').css('background-color');
         $('.kauction-container').css('background-color', bodyColor);
         if (this.$body.hasClass('sidebar-open')) {
             this.$body.removeClass('sidebar-open');
             timer = setTimeout(function() {
                 this.$element.removeClass('visible');
             }.bind(this), 400);
         } else {
             clearTimeout(timer);
             this.$element.addClass('visible');
             setTimeout(function() {
                 this.$body.addClass('sidebar-open');
             }.bind(this), 10);
             setTimeout(function(){
                $('.kauction-container').css({'background-color': ''});
             },1000);
         }
     }

     Sidebar.prototype.togglePinSidebar = function(toggle) {
         if (toggle == 'hide') {
             this.$body.removeClass('menu-pin');
         } else if (toggle == 'show') {
             this.$body.addClass('menu-pin');
         } else {
             this.$body.toggleClass('menu-pin');
         }
     }

     /* 플러그인 정의 */
     function Plugin(option) {
         return this.each(function() {
             var $this = $(this);
             var data = $this.data('ka.sidebar');
             var options = typeof option == 'object' && option;

             if (!data) $this.data('ka.sidebar', (data = new Sidebar(this, options)));
             if (typeof option == 'string') data[option]();
         })
     }

     var old = $.fn.sidebar;

     $.fn.sidebar = Plugin;
     $.fn.sidebar.Constructor = Sidebar;
     $.fn.sidebar.defaults = {
         kaContainer: '.kauction-container'
     }

     /* NO CONFLICT */
     $.fn.sidebar.noConflict = function() {
         $.fn.sidebar = old;
         return this;
     }

     /* DATA API */
     $(document).on('click.ka.sidebar.data-api', '[data-toggle-pin="sidebar"]', function(e) {
         var $this = $(this);
         var $target = $('[data-kauction="sidebar"]');
         $target.data('ka.sidebar').togglePinSidebar();
         return false;
     })
     $(document).on('click.ka.sidebar.data-api', '[data-toggle="sidebar"]', function(e) {
        console.log("menu open");
        var $this = $(this);
        var $target = $('[data-kauction="sidebar"]');
        $target.data('ka.sidebar').toggleSidebar();
        return false
    })

 })(window.jQuery);

/*------------------------------------------------------------------
 * Polyfill: 포커스
 * https://github.com/WICG/focus-ring
 * https://fvsch.com/styling-buttons/
 *------------------------------------------------------------------*/
(function (global, factory) {
	typeof exports === 'object' && typeof module !== 'undefined' ? factory() :
	typeof define === 'function' && define.amd ? define(factory) :
	(factory());
}(this, (function () { 
	
'use strict';

function init() {
  var hadKeyboardEvent = true;
  var hadFocusVisibleRecently = false;
  var hadFocusVisibleRecentlyTimeout = null;
  var inputTypesWhitelist = {
    text: true,
    search: true,
    url: true,
    tel: true,
    email: true,
    password: true,
    number: true,
    date: true,
    month: true,
    week: true,
    time: true,
    datetime: true,
    'datetime-local': true
  };

  /* 레거시 브라우저 헬퍼 */
  function isValidFocusTarget(el) {
    if (el && el !== document && el.nodeName !== 'HTML' && el.nodeName !== 'BODY') {
      return true;
    }
    return false;
  }

  /* :focus-visible */
  function focusTriggersKeyboardModality(el) {
    var type = el.type;
    var tagName = el.tagName;

    if (tagName == 'INPUT' && inputTypesWhitelist[type] && !el.readonly) {
      return true;
    }
    if (tagName == 'TEXTAREA' && !el.readonly) {
      return true;
    }
    if (el.contentEditable == 'true') {
      return true;
    }
    return false;
  }

  /* focus-visible 클래스 추가 */
  function addFocusVisibleClass(el) {
    if (el.classList.contains('focus-visible')) {
      return;
    }
    el.classList.add('focus-visible');
    el.setAttribute('data-focus-visible-added', '');
  }

  /* focus-visible 클래스 제거 */
  function removeFocusVisibleClass(el) {
    if (!el.hasAttribute('data-focus-visible-added')) {
      return;
    }
    el.classList.remove('focus-visible');
    el.removeAttribute('data-focus-visible-added');
  }

  /* 키다운 처리 */
  function onKeyDown(e) {
    if (isValidFocusTarget(document.activeElement)) {
      addFocusVisibleClass(document.activeElement);
    }
    hadKeyboardEvent = true;
  }

  /* 마우스 클릭 처리 */
  function onPointerDown(e) {
    hadKeyboardEvent = false;
  }

  /* 포커스 처리 */
  function onFocus(e) {
    // IE.. 
    if (!isValidFocusTarget(e.target)) {
      return;
    }

    if (hadKeyboardEvent || focusTriggersKeyboardModality(e.target)) {
      addFocusVisibleClass(e.target);
      hadKeyboardEvent = false;
    }
  }

  /* 블러 처리 */
  function onBlur(e) {
    if (!isValidFocusTarget(e.target)) {
      return;
    }

    if (e.target.classList.contains('focus-visible')) {
      // 탭/윈도우 스위치 검출
      hadFocusVisibleRecently = true;
      window.clearTimeout(hadFocusVisibleRecentlyTimeout);
      hadFocusVisibleRecentlyTimeout = window.setTimeout(function() {
        hadFocusVisibleRecently = false;
        window.clearTimeout(hadFocusVisibleRecentlyTimeout);
      }, 100);
      removeFocusVisibleClass(e.target);
    }
  }

  /* 탭 변경 처리 */
  function onVisibilityChange(e) {
    if (document.visibilityState == 'hidden') {
      if (hadFocusVisibleRecently) {
        hadKeyboardEvent = true;
      }
      addInitialPointerMoveListeners();
    }
  }

    /* 포인팅 디바이스 리스너 추가 */
  function addInitialPointerMoveListeners() {
    document.addEventListener('mousemove', onInitialPointerMove);
    document.addEventListener('mousedown', onInitialPointerMove);
    document.addEventListener('mouseup', onInitialPointerMove);
    document.addEventListener('pointermove', onInitialPointerMove);
    document.addEventListener('pointerdown', onInitialPointerMove);
    document.addEventListener('pointerup', onInitialPointerMove);
    document.addEventListener('touchmove', onInitialPointerMove);
    document.addEventListener('touchstart', onInitialPointerMove);
    document.addEventListener('touchend', onInitialPointerMove);
  }

  /* 포인팅 디바이스 리스너 제거 */
  function removeInitialPointerMoveListeners() {
    document.removeEventListener('mousemove', onInitialPointerMove);
    document.removeEventListener('mousedown', onInitialPointerMove);
    document.removeEventListener('mouseup', onInitialPointerMove);
    document.removeEventListener('pointermove', onInitialPointerMove);
    document.removeEventListener('pointerdown', onInitialPointerMove);
    document.removeEventListener('pointerup', onInitialPointerMove);
    document.removeEventListener('touchmove', onInitialPointerMove);
    document.removeEventListener('touchstart', onInitialPointerMove);
    document.removeEventListener('touchend', onInitialPointerMove);
  }

  /* 키보드 오프 */
  function onInitialPointerMove(e) {
    // 사파리..
    if (e.target.nodeName.toLowerCase() === 'html') {
      return;
    }

    hadKeyboardEvent = false;
    removeInitialPointerMoveListeners();
  }

  document.addEventListener('keydown', onKeyDown, true);
  document.addEventListener('mousedown', onPointerDown, true);
  document.addEventListener('pointerdown', onPointerDown, true);
  document.addEventListener('touchstart', onPointerDown, true);
  document.addEventListener('focus', onFocus, true);
  document.addEventListener('blur', onBlur, true);
  document.addEventListener('visibilitychange', onVisibilityChange, true);
  addInitialPointerMoveListeners();

  document.body.classList.add('js-focus-visible');
}

/* DOM */
function onDOMReady(callback) {
  var loaded;

  /* 콜백: 로드상태 */
  function load() {
    if (!loaded) {
      loaded = true;
      callback();
    }
  }

  if (document.readyState === 'complete') {
    callback();
  } else {
    loaded = false;
    document.addEventListener('DOMContentLoaded', load, false);
    window.addEventListener('load', load, false);
  }
}

if (typeof document !== 'undefined') {
  onDOMReady(init);
}

})));
(function($) {
    'use strict';
    // 레이아웃 및 플러그인 초기화
    (typeof angular === 'undefined') && $.KAuction.init();
})(window.jQuery);





