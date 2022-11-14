(function ($) {
    //'use strict';

    var didScroll;
    var lastScrollTop = 0;
    var delta = 5;
    var navbarHeight = $('.header').outerHeight();

    $(window).scroll(function (event) {
        didScroll = true;
    });

    // 모달 닫기
    let $modal = document.querySelector(".mypage_modal"),
        $body = document.querySelector('body'),
        $modalClose = document.querySelectorAll(".mypage_modal_close");

    window.addEventListener('click', (e) => {
        let target = e.target;
        
        if (
            target.classList.contains("mypage_pay_modal") || 
            target.classList.contains("pi_info_agree_modal") && !target.classList.contains("show")) {
            target.style.display = "none";
            $body.classList.remove('scroll_lock');
        }

        if (target.classList.contains("show")) {
            target.classList.remove('show');
            $body.classList.remove('scroll_lock');
        }
        

        if ($modalClose[0]) {
            $modalClose.forEach((el) => {
                el.addEventListener('click', (e) => {
                    let parent = e.currentTarget.parentNode.parentNode;

                    if(parent.classList.contains("show")) {
                        parent.classList.remove('show');
                    } else {
                        parent.style.display = "none";
                    }

                    $body.classList.remove('scroll_lock');
                });
            });
        }



    });


    // 카운트함수 - 아이폰에러 대응
    function convertDateForIos2(date) {
        var arr = date.split(/[- :]/);
        date = new Date(arr[0], arr[1]-1, arr[2], arr[3], arr[4], arr[5]);
        return date;
    }

    $.commonScript = {
        countDownTimer: function (type, date, id) {
            var _type = type;
            var _vDate = date;
            var _second = 1000;
            var _minute = _second * 60;
            var _hour = _minute * 60;
            var _day = _hour * 24;
            var timer;

            function showRemaining() {
                var now = new Date();
                var distDt = Date.parse(convertDateForIos2((_vDate))) - Date.parse(now);

                if (distDt < 0) {
                    clearInterval(timer);
                    return;
                }

                var days = Math.floor(distDt / _day);
                var hours = Math.floor((distDt % _day) / _hour) + (Math.floor(distDt / _day) * 24);
                var minutes = Math.floor((distDt % _hour) / _minute);
                var seconds = Math.floor((distDt % _minute) / _second);

                if (hours < 10) { hours = "0" + hours; }
                if (minutes < 10) { minutes = "0" + minutes; }
                if (seconds < 10) { seconds = "0" + seconds; }

                if (_type === "dayContDown") {
                    if (days < 1) {
                        if (hours < 1) {
                            document.getElementById(id).textContent = minutes + ' : ' + seconds;
                        } else {
                            document.getElementById(id).textContent = hours + ' : ' + minutes +' : ' + seconds;
                        }
                    } else {
                        document.getElementById(id).textContent = days;
                    }
                } else if (type === "hourContDown") {
                    document.getElementById(id).textContent = hours + ' : ';
                    document.getElementById(id).textContent += minutes + ' : ';
                    document.getElementById(id).textContent += seconds;
                }
            }

            if (document.getElementById(id)) {
                timer = setInterval(showRemaining, 1000);
            }
        }
    }

    function inputFocus (el) {
        this.$el = el;
        this.$parent = this.$el.parentElement;
        this.focusOn = function () {
            this.$el.addEventListener('focus', () => {
                this.$parent.style.borderColor = "#000"
            })
        }
        this.focusOff= function () {
            this.$el.addEventListener('blur', () => {
                this.$parent.style.borderColor = "#C1C1C1"
            })
        }
    }


    const fiterInput = document.querySelectorAll('.filter_wrap .bottom input');
    fiterInput.forEach(el => {
        let target = new inputFocus(el);
        target.focusOn();
        target.focusOff();
    });



    setInterval(function () {
        if (didScroll) {
            hasScrolled();
            didScroll = false;
        }
    }, 250);

    function hasScrolled() {
        var st = $(this).scrollTop();

        if (Math.abs(lastScrollTop - st) <= delta) return;
        if (st > lastScrollTop && st > navbarHeight) {
            $('.header').removeClass('header-down').addClass('header-up');
        } else {
            if (st + $(window).height() < $(document).height()) {
                $('.header').removeClass('header-up').addClass('header-down');
            }
        }
        lastScrollTop = st;
    };

    $(document).ready(function () {

        outdatedBrowser({
            bgColor: '#f25648',
            color: '#ffffff',
            lowerThan: 'transform',
            languagePath: 'assets/plugins/outdated/outdated.html'
        })


        // 3월30일수정
        $(function () {
            $(window).resize(function () {
                if (Modernizr.mq('(max-width: 1199.98px)')) {
                    $('.nav-menu-list').find('.nav-dropmenu-wrap').stop().hide();
                    $('.nav-dropmenu-wrap').height('auto');
                } else {
                    $('.nav-dropmenu-wrap').height('auto');
                }
            }).resize();
        });


        // 3월30일수정
        $('.nav-menu-list').on('click', function (e) {
            $('.nav-menu-list').find('> ul').removeClass('on').hide();

            // $(this).find('>ul').slideDown(500);
            $(this).find('>ul').css("display", "block")
        });

        // 4월6일수정
        const dropmenuItem = Array.prototype.slice.call(document.querySelectorAll('.nav-dropmenu-item'));
        dropmenuItem.forEach(function(elem) {
            if(elem.getAttribute("href") == window.location.pathname) {
                elem.style.borderLeft = "solid 5px #f0f0f0";
                elem.parentNode.parentNode.parentNode.parentNode.classList.add('on');
                
            }
        });


        // 9월27일수정
        // $('.btn-search').on('click', function () {
        //     if($('.search-container-wrap').css('display') === "none"){
        //         $('.header').addClass('searching');
        //         $('.header').dimBackground();
        //         $('.search-container-wrap').stop().slideDown(300);
        //         $('.search-input').focus();
        //         // $(this).prop('disabled', true);
        //     } else {
        //         $('.search-container-wrap').stop().slideUp(150);
        //         // $('.btn-search').prop('disabled', false);

        //         //에러메세지뜸
        //         $.undim();
        //     }
        // });


        //4월6일수정 배경클릭했을때 검색창 닫기
        $(document).mouseup(function (e) {
            var container = $(".search-container-wrap");
            if (container.has(e.target).length === 0) {
                $('.search-container-wrap').stop().slideUp(150);
                $.undim();
            }
        });

        $('.search-close').on('click touchstart', function () {
            $('.search-container-wrap').stop().slideUp(150);
            $('.btn-search').prop('disabled', false);
            $.undim();
        });

        $('.modal').on('show.bs.modal', function (e) {
            if ($(e.currentTarget).attr("data-popup")) {
                $("body").addClass("body-scrollable");
            }
        });

        $('.modal').on('hidden.bs.modal', function (e) {
            $("body").removeClass("body-scrollable");
            $(this).find('form').trigger('reset');
        });

        $(".list-view-wrapper").scrollbar();

        $("input[type=number]").on("onkeydown", function (e) {
            if (e.keyCode < 48 || e.keyCode > 57) {
                event.returnValue = false;
            }
        });

        $("input[type=tel]").on("onkeyup", function (e) {
            if (e.keyCode < 48 || e.keyCode > 57) {
                event.returnValue = false;
            }
        });

        const replaceChar = /[~!@\#$%^&*\()\-=+_'\;<>\/.\`:\"\\,\[\]?|{}]/gi;
        const replaceNotFullKorean = /[ㄱ-ㅎㅏ-ㅣ]/gi;
        const replaceNotFullEnglish = /[a-z]/gi;
        //$("input[type=tel]").on("focusout", function () {
        //    let x = $(this).val();
        //    if (x.length > 0) {
        //        if (x.match(replaceChar) || x.match(replaceNotFullKorean)) {
        //            x = x.replace(replaceChar, "").replace(replaceNotFullKorean, "");
        //        }
        //        $(this).val(x);
        //    }
        //}).on("keyup", function () {
        //    $(this).val($(this).val().replace(replaceChar, ""));
        //});
        $("input[type=tel]").on("keyup", function () {
            $(this).val($(this).val().replace(replaceChar, "").replace(replaceNotFullKorean, "").replace(replaceNotFullEnglish, ""));
        });
    });

    //$(window).on('popstate', function () {
    //    // History Check
    //    var hstate = history.state;
    //    if (hstate != null && hstate.formData) {
    //        // History 가 있을 경우 해당 FromData 로  HTML Pain
    //        $.paginationUtils.paintHTML(hstate.formData);
    //    } else {
    //        // History 가 없는 경우 Browser Back 기능
    //        history.back();
    //    }
    //});



    function inputClear (target) {
        let clear_Btn = `<img class="text_clear_btn" src="/img/icons/clear_btn.png" alt="clear-btn" srcset="/img/icons/clear_btn.png 1x, /img/icons/clear_btn@2x.png 2x, /img/icons/clear_btn@3x.png 3x">`;
        target.append(clear_Btn);
    }
    /* 클리어버튼 삽입 */
    $.fn.clearBtn = function() {
        this.each(function(index) {
            inputClear($(this)); 
        })
    }

    
    /* 인풋박스 CLEAR EVENT */
    $.fn.clearEvent = function() {
        this.each(function(index) {
            $(this).click(function(e){
                e.preventDefault();
                let input = $(this).siblings();
                input.val('');
                input.focus();
            })
        })
    }

    /* 셀렉트메뉴 */
    $.fn.selectMenuEvent = function(content) {
        let $content = null;
        if (!content && content == undefined) {
            $content = undefined;
        }
        // 탭메뉴 내용 대입
        $content = content;
 
        if($content) {
            $content.children().hide().eq(0).show();
        }
        this.each(function(index) {
            let selectTab = $(this);
            selectTab.on("change", function(){
                let idx = this.value;
                $content.children().hide().eq(idx).show();
            });
        });
    }


})(window.jQuery);



var imageObserver = new IntersectionObserver(function (entries, observer) {
    entries.forEach(function (entry) {
        if(!entry.isIntersecting) return;
        const target = entry.target;
        target.classList.remove('img-lazy');        
        
        try {
            // if (target.length !== 0) {
            if (target && target.getElementsByTagName("img").length > 0) {
                /*const child = target.firstElementChild;*/
                /*child.src = child.dataset.src*/
                const child = target.getElementsByTagName("img")[0];
                child.src = child.dataset.src;
            }
        } catch (e) { }
        observer.unobserve(target);
    }, { threshold : 0.5 });
});

const skeletonLoadFunc = (targets) => {
    targets.forEach(function (el) {
        imageObserver.observe(el);
    });
}

