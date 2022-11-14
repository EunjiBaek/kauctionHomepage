var mobile = (/iphone|ipad|ipod|android/i.test(navigator.userAgent.toLowerCase()));

if (mobile) {
    var userAgent = navigator.userAgent.toLowerCase();
    if (userAgent.search("android") > -1) {
        currentOS = "android";
        $(".change-list.mb > img").attr("src", "/img/temp/highlight/android-ico.png");
        $(".change-list.mb > img").attr("onclick", "window.open('https://play.google.com/store/apps/details?id=com.kauctionapp.kauction', '_blank');");
    } else if ((userAgent.search("iphone") > -1) || (userAgent.search("ipod") > -1) || (userAgent.search("ipad") > -1)) {
        currentOS = "ios";
        $(".change-list.mb > img").attr("src", "/img/temp/highlight/ios-ico.png");
        $(".change-list.mb > img").attr("onclick", "window.open('https://apps.apple.com/kr/app/kbs-kong/id397892527', '_blank');");
    }
}

function isMobile() {
    return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
}

if (isMobile() && checkPage()) {
    document.querySelector('#back-btn').style.display = 'block';
    
} else {
    document.querySelector('#back-btn').style.display = 'none';
}

function checkPage() {
    return !(window.location.pathname.toLowerCase().indexOf('/member/join') > -1 || window.location.pathname.toLowerCase().indexOf('/member/joincomplete') > -1);
}

$(window).resize(function () {
    const wwid = $(this).width();
    if (wwid <= 768) {
        $(".change-list.cl5 > p").text(ka.msg.main.consignMobile);
    } else {
        $(".change-list.cl5 > p").text(ka.msg.main.consign);
    }
});

let noticeRollingOff;

$(document).ready(function () {
    var height = $("#change-notice > div > .notice").height() > 0 ? $("#change-notice > div > .notice").height() : 50;
    var num = $("#change-notice > div > .notice > .rolling li").length;
    var max = height * num;
    var move = 0;
    function noticeRolling() {
        move += height;
        $("#change-notice > div > .notice > .rolling").animate({ "top": -move }, 1000, function () {
            if (move >= max) {
                jQuery(this).css("top", 0);
                move = 0;
            };
        });
    };

    if (num > 1) {
        noticeRollingOff = setInterval(noticeRolling, 5000);
        $("#change-notice > div > .notice > .rolling").append($(".rolling li").first().clone());
    }

    $("#change-notice > div > .notice").hover(function () {
        clearInterval(noticeRollingOff);
    }, function () {
        noticeRollingOff = setInterval(noticeRolling, 5000);
    });


    let quickMenuClick = false,
        $quick_menu = document.getElementById('quick-menu'),
        $mb_consign_btn = document.getElementById('mb-consign-btn'),
        $quick_menu_cnt = document.getElementById('quick-menu-cnt'),
        $quick_menu_close_btn = document.querySelector("#quick-menu-cnt > .quick-menu-cnt-close > div");

    // 앱스토어 큐알코드 팝업창
    // document.getElementById('appdown_btn').addEventListener('click', () => {
    //     document.querySelector('.modal.qrcode').classList.toggle('show')
    //     document.querySelector('body').classList.toggle('scroll_lock')
    // });


    function quickmenu_mediaquery() {
        if (matchMedia("screen and (max-width: 1430px)").matches) {
            if (!$quick_menu.classList.contains('active')) {
                $quick_menu.classList.add('active');
                $mb_consign_btn.classList.add('active');
            }
            $quick_menu_cnt.classList.remove('active');
        } else {
            if ($quick_menu.classList.contains('active')) {
                $quick_menu.classList.remove('active');
                $mb_consign_btn.classList.remove('active');
            }

            if (
                !$quick_menu_cnt.classList.contains('active')) {
                $quick_menu_cnt.classList.add('active');
            }

        }
    }

    window.addEventListener('resize', function(){
        quickmenu_mediaquery()
    });

    quickmenu_mediaquery();


    $quick_menu.addEventListener('mouseenter', (e) => {
        const cta = e.currentTarget.getElementsByClassName('cta')[0].children;
        cta.forEach(el => {
            el.classList.remove('bounceAlpha');
        })
    });

    $quick_menu.addEventListener('mouseleave', (e) => {
        const cta = e.currentTarget.getElementsByClassName('cta')[0].children;
        cta.forEach(el => {
            el.classList.add('bounceAlpha');
        })
    });

    $quick_menu.addEventListener('click', (e) => {
        e.currentTarget.classList.remove('active');
        $mb_consign_btn.classList.remove('active');
        if(!quickMenuClick) {
            $quick_menu_cnt.classList.add('active');
            quickMenuClick = true;
        }
    });

    $quick_menu_close_btn.addEventListener('click', () => {
        $quick_menu_cnt.classList.remove('active');
        setTimeout(() => {
            $quick_menu.classList.add('active');
            $mb_consign_btn.classList.add('active');
        }, 100);
        quickMenuClick = false;
    });

    document.querySelectorAll("#quick-menu-cnt > .auction > .cnt > div").forEach(el => {
        el.addEventListener('mouseenter', (e) => {
            let $target = e.currentTarget,
                $auc_state = $target.getElementsByClassName('auc_state')[0];
            $target.classList.add('on');
            $auc_state.style.display = "block";
            
        });

        el.addEventListener('mouseleave', (e) => {
            let $target = e.currentTarget,
                $auc_state = $target.getElementsByClassName('auc_state')[0];
            $target.classList.remove('on');
            $auc_state.style.display = "none";
            
        });
    });


});

var cloneElements = $('.notice').clone();
cloneElements.appendTo('.noticedesc');

$.each(cloneElements.find("span"), function (index, item) {
    var click = item.dataset.click;
    if (typeof click === "string" && click !== "") {
        item.setAttribute("onclick", click);
    }
});

const vimg = $('<img>', {
    'src': '/img/icons/bullet.png',
    'max-width': '100%',
    'height': 'auto'
});
$(vimg).prependTo('#notice-popup > .noticedesc > .notice > ul > li');

const noticeList = Array.prototype.slice.call(document.querySelectorAll('.rolling > li'));

for (i = 0; i < noticeList.length; i++) {
    if (noticeList[i].getElementsByTagName('span').length > 0) {
        const n = document.createElement('a');
        noticeList[i].appendChild(n);
        const spanTag = n.previousElementSibling;
        n.appendChild(spanTag);
    }
}

const linkIcon = document.createElement('i');
linkIcon.className = "fas fa-link";
linkIcon.style.fontSize = "11px";
linkIcon.style.paddingLeft = "6px";

if (document.querySelectorAll('#notice-popup > .noticedesc > .notice > ul > li > a') !== null) {
    const noticeA = document.querySelectorAll('#notice-popup > .noticedesc > .notice > ul > li > a');
    for (let i = 0; i < noticeA.length; i++) {
        noticeA[i].href = noticeA[i].firstElementChild.getAttribute('value');
        noticeA[i].firstElementChild.removeAttribute('onclick');
        noticeA[i].firstElementChild.removeAttribute('data-click');

        const linkTconClone = linkIcon.cloneNode();
        noticeA[i].appendChild(linkTconClone);
    }
}

if (document.querySelectorAll('#change-notice > div > .notice > ul > li > a') !== null) {
    const changeNotice = document.querySelectorAll('#change-notice > div > .notice > ul > li > a');
    for (let i = 0; i < changeNotice.length; i++) {
        const linkTconClone = linkIcon.cloneNode();
        changeNotice[i].appendChild(linkTconClone);
    }
}

$("#change-notice > div > .notice > ul > li").click(function (e) {
    const idx = Array.from($("#change-notice > div > .notice > ul > li")).indexOf(e.currentTarget);
    $('#notice-popup > .noticedesc > .notice > ul > li > a').eq(idx).css({
        "color": '#02328a'
    });
    $('#notice-popup > .noticedesc > .notice > ul > li').eq(idx).css({
        "color": '#02328a'
    });

    e.preventDefault();
    $("#notice-popup").css("display", "block");
    $("body").css({ "overflow-y": "hidden", "height": "100%", "touch-action": "none" }).bind('scroll touchmove mousewheel', function (e) { e.preventDefault() });
});

$("#notice-popup > .noticedesc > .notice-close").click(function (e) {
    e.preventDefault();
    $("#notice-popup").css("display", "none");
    $("body").css({ "overflow-y": 'scroll', "position": "static", "touch-action": "auto" }).unbind('scroll touchmove mousewheel');

    $('#notice-popup > .noticedesc > .notice > ul > li > a').css({
        "color": '#000000'
    });
    $('#notice-popup > .noticedesc > .notice > ul > li').css({
        "color": '#000000'
    });
});

$("#notice-popup > .noticebg").click(function (e) {
    e.preventDefault();
    $("#notice-popup").css("display", "none");
    $("body").css({ "overflow-y": 'scroll', "position": "static", "touch-action": "auto" }).unbind('scroll touchmove mousewheel');
});

$(".todaycl-btn > a:first-child").click(function (e) {
    e.preventDefault();
    const topNotice = $("#change-notice");
    if (topNotice.length > 0) {
        $.commonUtils.setCookie00("ka-" + topNotice.get(0).dataset.id, "Y", 1);
    }
    topNotice.hide();
});

$(".todaycl-btn > a:last-child").click(function (e) {
    e.preventDefault();
    $("#change-notice").hide();
});

$(".today-close").on('click', function () {
    $('.todaycl-btn').css({ "display": "flex" });
});

$("body").on("click", function (e) {
    if (!$("#change-notice").has(e.target).length) {
        $(".todaycl-btn").css({ "display": "none" });
    }
});

const hllist = $(".hllilst > li").length;
const hlistindex = $(".hllilst > li").index();

if (hllist >= 14) {
    $(".img-more").css("display", "inline-block");
    $(".hllilst > li:nth-of-type(n+15)").css("display", "none");
}


$(".img-more").click(function (e) {
    e.preventDefault();
    $(this).css("display", "none");
    $(".hllilst > li:nth-of-type(n+15)").css("display", "inline-block");
});




// 하이라이트 탭 바꾸기
const hlbtn = $(".hl-btn > ul > li");
const hlCont = $(".hl-list > div");

hlCont.hide().eq(0).show();
hlbtn.click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    document.querySelector('.work-sns > .slick-list').classList.add('on');
    var target = $(this);
    var index = target.index();

    $('.work-sns .slick-slide').css({ "height": "0" });
    $('.work-sns .slick-current').css({ "height": "100%" });
    hlbtn.removeClass("active");
    target.addClass("active");
    hlCont.css("display", "none");
    hlCont.eq(index).css("display", "block");

});



$(".hllilst > li > div > svg").on("click", function (e) {
    var el = e.target.parentNode.parentNode.tagName == "DIV" ? e.target.parentNode.parentNode : e.target.parentNode;
    if (el.dataset.wishYn === "Y") {
        if ($.wishlist.add(el.dataset.uid, 'N', false)) {
            for (var i = 0; i < el.children.length; i++) {
                if (el.children[i].tagName === "svg") {
                    el.children[i].setAttribute("class", "");
                    el.dataset.wishYn = "N";
                }
            }
        }
    } else {
        if ($.wishlist.add(el.dataset.uid, 'Y', false)) {
            for (var i = 0; i < el.children.length; i++) {
                if (el.children[i].tagName === "svg") {
                    el.children[i].setAttribute("class", "open");
                    el.dataset.wishYn = "Y";
                }
            }
        }
    }
    return false;
});


function deviceCheckon() {
    // 디바이스 종류 설정
    var pcDevice = "win16|win32|win64|mac|macintel";
 
    // 접속한 디바이스 환경
    if ( navigator.platform ) {
        if ( pcDevice.indexOf(navigator.platform.toLowerCase()) < 0 ) {
            document.querySelector(".swiper-container.main > .swiper-wrapper > .swiper-slide > .banner-img > img").classList.remove('on');
        } else {
            document.querySelector(".swiper-container.main > .swiper-wrapper > .swiper-slide > .banner-img > img").classList.add('on');
        }
    }
}


// 타블렛 이미지 사이즈
function isTabletDevice() { 
    let slideImg = document.querySelectorAll(".swiper-container.main > .swiper-wrapper > .swiper-slide > .banner-img > img");

    if (navigator.userAgent.toLowerCase().indexOf('ipad') > -1 || (navigator.userAgent.toLowerCase().indexOf('android') > -1 && navigator.userAgent.toLowerCase().indexOf('mobile') == -1)) {
        for(let i=0; i<slideImg.length; i++){
            slideImg[i].classList.add("on");
        }
    }else {
        for(let i=0; i<slideImg.length; i++){
            slideImg[i].classList.remove("on");
        }
    }
}

isTabletDevice();


let swiperOptions = {};
if ($(".swiper-container .swiper-slide").length == 1) {
    swiperOptions = {
        effect: 'fade',
        autoplay: false,
        loop: false,
        pagination: {
            el: '.swiper-pagination',
            clickable: false,
        },
        observer: true,
        observeParents: true,
        lazyLoading: true,
        navigation: {
            nextEl: '.swiper-button-next',
            prevEl: '.swiper-button-prev',
        }
    }
} else {
    swiperOptions = {
        autoHeight: true,
        effect: 'fade',
        speed: 1000,
        autoplay: {
            delay: 9000,
            disableOnInteraction: false,
        },
        slidesPerView: 1,
        spaceBetween: 30,
        loop: true,
        pagination: {
            el: '.swiper-pagination',
            clickable: true,
        },
        observer: true,
        observeParents: true,
        lazyLoading: true,
        navigation: {
            nextEl: '.swiper-button-next',
            prevEl: '.swiper-button-prev',
        },
        on: {
            slideChange: function () {
                startProgressbar();
            }
        }
        // on: {
        //     init: function(){
        //         startProgressbar();
        //     }
        // }
    }
}

$(".stop.s1").on('click', function (e) {
    swiper.autoplay.stop();
    resetProgressbar()
    $(this).removeClass("on").next().addClass("on");
});

$(".stop.s2").on('click', function (e) {
    swiper.autoplay.start();
    resetProgressbar();
    tick = setInterval(interval, 10);
    $(this).removeClass("on").prev().addClass("on");
});

var swiper = null;

$(document).ready(function () {
    swiper = new Swiper('.swiper-container.main', swiperOptions);
});

const time = 8.5;
let tick,
    percentTime;

startProgressbar();

function startProgressbar() {
    resetProgressbar();
    percentTime = 0;
    tick = setInterval(interval, 10);
}

function interval() {
    percentTime += 1 / (time + 0.1);

    $('.swiper-slide-active .hero_slide--progress').css({
        width: percentTime + "%"
    });
    if (percentTime >= 100) {
        if ($('.swiper-slide-active .hero_slide--progress').css('display') == 'block') {
            swiper.slideNext();
        }
        startProgressbar();
    }
}

function resetProgressbar() {
    clearTimeout(tick);
}

function navigationFunc() {
    $('.hero_slide--progress').css({ width: 0 + '%' });
    startProgressbar();
    swiper.autoplay.start();
    $(".stop.s2").removeClass('on').prev().addClass("on");
}

$('.swiper-button-prev').click(function (e) {
    e.preventDefault();
    navigationFunc();
});

$('.swiper-button-next').click(function (e) {
    e.preventDefault();
    navigationFunc();
});

window.addEventListener('scroll', function () {
    scrollTop = document.documentElement.scrollTop;
    try {
        if (typeof swiper.autoplay !== "undefined") {
            if (scrollTop > window.outerHeight / 1.5) {
                swiper.autoplay.stop();
            } else {
                swiper.autoplay.start();
            }
        }
    } catch (e) { }
});

$("#alltop-btn > .topbtn").on('click', function (e) {
    e.preventDefault();
    $('body, html').animate({
        scrollTop: 0
    }, 800);
});

$("#alltop-btn > .bottombtn").on('click', function (e) {
    e.preventDefault();
    $('body, html').animate({
        scrollTop: $(document).height()
    }, 800);
});

$("#maintop-btn").on('click', function (e) {
    e.preventDefault();
    $('body, html').animate({
        scrollTop: 0
    }, 800);
});

var selectTab = document.getElementById("Select");
var con = $(".auc-desc > div");
con.hide().eq(0).show();

if (selectTab !== null) {
    selectTab.addEventListener("change", function () {
        var val = selectTab.options[selectTab.selectedIndex].value;
        for (var i = 0; i < selectTab.length; i++) {
            con[i].style.display = "none";

            if (val == i) {
                con[i].style.display = "block";

            } else if (val == "x") {
                con[0].style.display = "block";
                document.getElementById("tab1").selected = true;
            }
        }
    });
}

if (hlistindex >= 14) {
    $(".hllilst > li").addClass("margint");
}

$(window).resize(function () {
    var windowWidth = $(window).width();
    if (windowWidth < 1020) {
        $("#auction-rel > .schedule > div > .tit > h4").text(ka.msg.main.schedule2);
    } else {
        $("#auction-rel > .schedule > div > .tit > h4").text(ka.msg.main.schedule);
    }
});

$(".search-input").keydown(function (key) {
    if (key.keyCode == 13) {
        if (key.target.value.replace(/ /ig, "") === "") return false;

        // 2022.03.23 :: Test
        // $.searchUtils.setSearchKey("T", key.target.value, false);

        window.location.href = "/Home/Search?key=" + encodeURI(key.target.value);
    }
});

const listimg = Array.prototype.slice.call(document.querySelectorAll('.artwork > a'));

for (let listImgIndex = 0; listImgIndex < listimg.length; listImgIndex++) {
    listimg[listImgIndex].className = 'listimg';
}

$('.text-truncate').each(function () {
    if (this.offsetWidth < this.scrollWidth) {
        $(this).attr('title', $(this).text());
    }
});

$('#main-popup').mouseup(function (e) {
    var container = $(".mainpopup");
    if (container.has(e.target).length === 0) {
        if (document.querySelector('#main-popup') !== null) {
            document.querySelector('#main-popup').style.display = "none";
        }
        if (document.querySelector('.mainpopup-bg') !== null) {
            document.querySelector('.mainpopup-bg').style.display = "none";
        }
        $("body").css({ "overflow-y": 'scroll', "position": "static", "touch-action": "auto" }).unbind('scroll touchmove mousewheel');
    }
});

function bodyHidden() {
    $("body").bind('scroll touchmove mousewheel', function (e) { e.preventDefault() }, {passive: false});
}

$('.card-price-btn > a').click(function (e) {
    e.preventDefault();
    $('#modal-bid-noti').addClass('block');
    $('body').addClass('yhidden');
    bodyHidden();
    $('.modal-bg').addClass('block');
    return;
});

function modalDocumentEvidence(target) {
    document.querySelector('body').classList.add('yhidden');
    if (document.querySelector('#modal-document-evidence') !== null) {
        document.querySelector('#modal-document-evidence').classList.add('block');
    }
    document.querySelector('.modal-bg').classList.add('block');

    if (isKor === "False") {
        $(".tab-international").click();
    }

    if (verifyRequestExistYn === 'Y') {
        $.commonUtils.alert(ka.msg.common.verificationAlreadyExistAlert);
    }

    document.querySelector('#modal-show-popup-btn').onclick = function () {        
        modalDocumentEvidenceRemove();
        $('#' + target).click();
        return false;
    }

    document.querySelector('.modal-evidence-close').onclick = function () {
        modalDocumentEvidenceRemove();
        return false;
    }
}
function modalDocumentFile(target) {
    $('#' + target).click();
}


window.onclick = function (event) {

    try {
        if (event.target.parentNode.classList.length != null && event.target.parentNode.classList.length > 0) {
            const hasClass = event.target.parentNode.classList.contains('modal-show-popup');
            if (hasClass) {
                event.target.parentNode.classList.remove('block');
                document.querySelector('body').classList.remove('yhidden');
    
                if (document.querySelector('.modal-bg') !== null) {
                    document.querySelector('.modal-bg').classList.remove('block');
                }
                if (document.querySelector('.popupbg') !== null) {
                    document.querySelector('.popupbg').classList.remove('block');
                }
    
                if (event.target.parentNode.classList.contains('bp02')) {
                    if (document.body.clientWidth > 768) {
                        document.querySelector('body').classList.remove('yhidden');
                    } else {
                        document.querySelector('body').classList.add('yhidden');
                    }
                }
            }
        }
    } catch (error) {

    }
};

function modalDocumentEvidenceRemove() {
    document.querySelector('body').classList.remove('yhidden');
    document.querySelector('#modal-document-evidence').classList.remove('block');
    document.querySelector('.modal-bg').classList.remove('block');
}

const replaceChar = /[~!@\#$%^&*\()\=+_'\;<>\/.\`:\"\\,\[\]?|{}]/gi;
const replaceNotFullKorean = /[ㄱ-ㅎㅏ-ㅣ]/gi;
$(document).ready(function () {
    $("#modal-address2").on("focusout", function () {
        let x = $(this).val();
        if (x.length > 0) {
            if (x.match(replaceChar) || x.match(replaceNotFullKorean)) {
                x = x.replace(replaceChar, "").replace(replaceNotFullKorean, "");
            }
            $(this).val(x);
        }
    }).on("keyup", function () {
        $(this).val($(this).val().replace(replaceChar, ""));
    });
});

function onlyNumber(event) {
    return event.charCode >= 48 && event.charCode <= 57;
}

var mobileKeyWords = new Array('iPhone', 'iPod', 'BlackBerry', 'Android', 'Windows CE', 'Windows CE;', 'LG', 'MOT', 'SAMSUNG', 'SonyEricsson', 'Mobile', 'Symbian', 'Opera Mobi', 'Opera Mini', 'IEmobile');
function pcviewBtn() {
    for (var word in mobileKeyWords) {
        if (navigator.userAgent.match(mobileKeyWords[word]) != null) {
            document.querySelector('#metaviewport').setAttribute(
                'content', 'width=1280, user-scalable=yes');
            document.querySelector('body').style.display = "block";
            break;
        }
    }
};

const formCheck = Array.prototype.slice.call(document.querySelectorAll('.form-check-choice > label'));
formCheck.forEach(function (elem) {
    elem.addEventListener('click', function (e) {
        const cardDefault = Array.prototype.slice.call(document.querySelectorAll('.card-default'));
        cardDefault.forEach(function (card) {
            card.classList.remove('show');
        });
        elem.parentNode.parentNode.parentNode.parentNode.classList.add('show');
    });
});

const aAfter = Array.prototype.slice.call(document.querySelectorAll('a.collapsed'));
aAfter.forEach(function (elem) {
    elem.addEventListener('click', function () {
        const f = document.querySelector('.collapse.show');
        if (!f) {
            elem.parentNode.parentNode.parentNode.classList.add('show');
        } else {
            elem.parentNode.parentNode.parentNode.classList.remove('show');
        }
    });
});

$(document).ready(function () {
    const agent = navigator.userAgent.toLowerCase();
    if ((navigator.appName == 'Netscape' && navigator.userAgent.search('Trident') != -1) || (agent.indexOf("msie") != -1)) {
        temporaryPopup();
    }

    function temporaryPopup() {
        document.querySelector('#temporary-popup').classList.add('block');
    }
});

function temporaryPopupClose() {
    document.querySelector('#temporary-popup').classList.remove('block');
    return;
}

const mqlsize = window.matchMedia("screen and (max-width: 1200px)");
const bannerImg = $('.swiper-container.main > .swiper-wrapper > .swiper-slide > .banner-img > img');

function deviceCheck() {
    var pcDevice = "win16|win32|win64|mac|macintel";
    if (navigator.platform) {
        if (pcDevice.indexOf(navigator.platform.toLowerCase()) < 0) {
            if (mqlsize.matches) {
                bannerImg.css({
                    "width": "auto",
                    "height": "100%"
                })
            } else {
                bannerImg.css({
                    "width": "auto",
                    "height": "auto"
                })
            }
        } else {
            if (mqlsize.matches) {
                bannerImg.css({
                    "width": "100%",
                    "height": "auto"
                })
            } else {
                bannerImg.css({
                    "width": "auto",
                    "height": "auto"
                })
            }
        }
    }
}


// 랏번호 검색 스크립트 (검색어필터)
function filter(id, inner, arr) {
    let search = id.value.toString();
    let search2 = id.value.toLowerCase();
    let search3 = id.value.replace(/(\s*)/g, "");

    let listInner = inner;
    let lotarr = arr;

    for(let i = 0; i < listInner.length; i++){
        lotNum = lotarr[i].getElementsByClassName("listLot");
        if(lotNum[0].innerHTML.toString().includes(search) || lotNum[0].innerHTML.toLowerCase().includes(search2) || lotNum[0].innerHTML.toLowerCase().replace(/(\s*)/g, "").includes(search3)){
            listInner[i].style.display = "block"
        }else{
            listInner[i].style.display = "none"
        }
    }
}


// 랏번호 드롭다운메뉴 클릭시 show
function dropdownClick (Btn, Dropdown) {
    const lotDropdown = Dropdown;
    const lotBtn = Btn;
    lotBtn.addEventListener('click', function(e){
        e.preventDefault();
        e.stopPropagation();
        lotDropdown.classList.toggle('showdrop');

        // 해당랏에 포커싱
        (function (){
            if(document.querySelector('.listInner.active') !== undefined && document.querySelector('.listInner.active') !== null){
                document.querySelector('.listBox').scrollTop = document.querySelector('.listInner.active').offsetTop - 50
            }
        })();

        if(document.querySelector('.top-lot-num > i') !== null) {
            //드롭다운메뉴 arrow shape 변경
            if(lotDropdown.classList.contains('showdrop')){
                document.querySelector('.top-lot-num > i').className = "fas fa-chevron-up";
            }else{
                document.querySelector('.top-lot-num > i').className = "fas fa-chevron-down";
            }
        }

    });
}


// 응찰페이지 lot검색
if(document.querySelector('.top-center .top-lot-num') !== null && document.querySelector("#search-input") !== null) {
    dropdownClick(document.querySelector('.top-center .top-lot-num'), document.querySelector('.top-center > .lot-dropdown'));
    
    // 임시
    document.querySelector("#search-input").addEventListener('keyup', function(e){
        filter(document.getElementById("search-input"), document.getElementsByClassName("listInner"), document.getElementsByClassName("lotarr"));
    });

}


// 리스트페이지 lot검색
try {
    dropdownClick(document.querySelector('.lot-search-wrap'), document.querySelector('.card-default-lot-search > .lot-dropdown'));

    document.querySelector("#search-input_list").addEventListener('keyup', function(e){
        filter(document.getElementById("search-input_list"), document.getElementsByClassName("listInner"), document.getElementsByClassName("lotarr"));
    });
} catch (e) {

}


// 드롭다운메뉴 리스트에 포커싱
function lotActive(num, arr) {
    let lotNum = num;
    let listLotArr = Array.prototype.slice.call(arr);
    let result = listLotArr.filter(x => {
        return x.innerHTML.toLowerCase().replace(/(\s*)/g, "") == lotNum.textContent.toLowerCase().replace(/(\s*)/g, "");
    });
    if (result[0]) {
        result[0].parentElement.parentElement.classList.add('active');
    }
}


// work.cshtml 드롭다운메뉴 리스트에 포커싱
if (document.querySelector('.top-lot-num > span') !== null) {
    lotActive(document.querySelector('.top-lot-num > span'), document.querySelectorAll('.listLot'));
}


// 배경클릭시 드롭다운메뉴 닫기
function dropClose(event) {
    const lotDropdown = document.querySelector('.lot-dropdown');

    const eTarget = event.target;

    try {
        if(eTarget.classList.contains('lot-dropdown') || eTarget.parentNode.classList.contains('lot-dropdown') || eTarget.parentNode.parentNode.classList.contains('lot-dropdown') || eTarget.parentNode.parentNode.parentNode.classList.contains('lot-dropdown')){
        
        }else{
            if(lotDropdown !== null && lotDropdown.classList.contains('showdrop')) {
                lotDropdown.classList.remove('showdrop');
            }
            if (document.querySelector('.top-lot-num > i') !== null) {
                document.querySelector('.top-lot-num > i').className = "fas fa-chevron-down";
            }
        }
    } catch(error) {

    }
}

window.addEventListener('click', dropClose);



