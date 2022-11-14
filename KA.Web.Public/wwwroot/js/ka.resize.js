
$(window).resize(function () {
    const windowWidth = $(window).width();
    const topcrollbtn = $(".top-scrollbtn");

    if (windowWidth > 1200) {
        if ($(".card-empty").length > 0) {
            $(".card-empty").show();
        }

        if ($(".pagination").length > 0) {
            $.paginationUtils.reset(10);
        }

        if (document.querySelector('#mobilebid') !== null) {
            document.querySelector('#mobilebid').classList.remove('block');
        }

        $('body').removeClass('yhidden');
    }

    //4월23일수정 swiper autoHeight값 변경 .swiper-container.main > .swiper-wrapper.autoHeight
    if(document.querySelector('.swiper-container.main > .swiper-wrapper') !== null){
        if (windowWidth > 1200) {
            document.querySelector('.swiper-container.main > .swiper-wrapper').classList.add('autoHeight');
        }else{
            document.querySelector('.swiper-container.main > .swiper-wrapper').classList.remove('autoHeight');
        }
    }

    if (windowWidth > 768) {
        if ($(".card-empty").length > 0) {
            $(".card-empty").show();
        }

        if ($(".pagination").length > 0) {
            $.paginationUtils.reset(10);
        }

        if ($('.nav-auction').length > 0) {
            $.each($('.nav-auction').find("a"), function (index, item) {
                if (typeof item.dataset.titleDefault === "string") {
                    item.innerHTML = item.dataset.titleDefault;
                    if (typeof item.dataset.count === "string" && item.dataset.count !== "") {
                        item.innerHTML += " <span>(" + item.dataset.count + ")</span>"
                    }
                }
            });
        }


        // $(".slide-all > div > div").hide();
        // $(".slide-all > div > div").eq(0).show();

        // var oldidx = 0;
        // var idx = 0;

        // function changeImg(idx) {
        //     if (oldidx != idx) {
        //         $(".image-list ul > li").eq(idx).addClass("active");
        //         $(".image-list ul > li").eq(oldidx).removeClass("active");

        //         $(".slide-all > div > div").eq(idx).stop(true, true).show();
        //         $(".slide-all > div > div").eq(oldidx).stop(true, true).hide();
        //     }
        //     oldidx = idx;
        // }

        // $(".image-list ul > li").on("click", function (e) {
        //     idx = $(this).index();
        //     changeImg(idx);

        //     e.preventDefault();
        //     e.stopPropagation();
        //     e.stopPropagation();
        // });

        $('.test').unbind();

        topcrollbtn.removeClass("open");

        $(".btn-list").off("click");

    } else {
        if ($(".card-empty").length > 0) {
            $(".card-empty").hide();
        }

        if ($(".pagination").length > 0) {
            $.paginationUtils.reset(5);
        }

        if ($('.nav-auction').length > 0) {
            $.each($('.nav-auction').find("a"), function (index, item) {
                if (typeof item.dataset.titleMobile === "string") {
                    item.innerHTML = item.dataset.titleMobile;
                    if (typeof item.dataset.count === "string" && item.dataset.count !== "") {
                        item.innerHTML += " <span>(" + item.dataset.count + ")</span>"
                    }
                }
            });
        }

        $(".image-list ul > li").off("click");
        $(".slide-all > div > div").show();

        topcrollbtn.addClass("open");
        $(window).bind("scroll.test", function () {
            let wScroll = $(window).scrollTop();
            if ($(".left .right").length > 0 && wScroll >= $(".left .right").offset().top) {
                topcrollbtn.css("opacity", "1")
            } else {
                topcrollbtn.css("opacity", "0")
            }
        });

        $(".btn-list").on("click", function (e) {
            $("#btn-all-list").click();
        });
    }
    
     // list
    // if (Modernizr.mq('(max-width: 1199.98px)')) {
    //     $('.nav-menu-list').find('.nav-dropmenu-wrap').stop().hide();
    //     $('.nav-dropmenu-wrap').height('auto');
    // } else {
    //     $('.nav-dropmenu-wrap').height('auto');
    // }

    if (window.matchMedia('(orientation: portrait)').matches) {
        $(".mobile-bidbtn.m1").off("click.mb");
        $(".bid-listmb").show();
        $(".bidbtn-bg > p").css("display", "none");
    } else {
        if (document.querySelector('#mobilebid') !== null) {
            document.querySelector('#mobilebid').classList.remove('block');
        }

        $(".mobile-bidbtn.m1").on("click.mb", function () {
            $(".bidbtn-bg > p").css("display", "block");
            $(".bid-listmb").hide();
        });

        $("#bidlistmb-close").on("click", function () {
            $(".bidbtn-bg > p").css("display", "none");
        });
    }

    if (window.innerWidth <= window.innerHeight) {
        // Portrait
        $(".mobile-bidbtn.m1").off("click.mb");
        $(".bid-listmb").show();
        $(".bidbtn-bg > p").css("display", "none");
    } else {
        if (document.querySelector('#mobilebid') !== null) {
            document.querySelector('#mobilebid').classList.remove('block');
        }

        $(".mobile-bidbtn.m1").on("click.mb", function () {
            $(".bidbtn-bg > p").css("display", "block");
            $(".bid-listmb").hide();
        });

        $("#bidlistmb-close").on("click", function () {
            $(".bidbtn-bg > p").css("display", "none");
        });
    }
});

// 바디스크롤 제어
const targetElement = $('body');
function overflowHidden(Element) {
    Element.css({
        "overflow-y": "hidden",
        "height": "100%",
        "touch-action": "none"
    }).bind('scroll touchmove mousewheel',
        function (e) { e.preventDefault() });
}

function overflowScroll(Element) {
    Element.css({
        "overflow-y": 'scroll',
        "position": "static",
        "touch-action": "auto"
    }).unbind('scroll touchmove mousewheel');
}