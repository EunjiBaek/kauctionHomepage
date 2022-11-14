$(function () {
    $('.share > a').on("click", function (e) {
        e.preventDefault();
    });
});

function copy_to_clipboard() {
    const loc = document.location;
    var link = loc.protocol + "//" + loc.host + loc.pathname + (isKor !== "True" ? "?lang=en" : "");
    $("#my-url").val(link);

    const copyText = document.getElementById("my-url");
    const textarea = document.createElement('textarea');
    document.body.appendChild(textarea);
    textarea.value = copyText.value;
    textarea.select();
    document.execCommand('copy');
    document.body.removeChild(textarea);
    if (typeof isMobile === "string" && isMobile === "True") {
        $.commonUtils.alert(ka.msg.auction.clipboardMobile, 'success');
    } else {
        $.commonUtils.alert(ka.msg.auction.clipboard, 'success');
    }
}

function Refresh() {
    window.parent.location = window.parent.location.href;
}

$(".top-scrollbtn").on('click', function (e) {
    e.preventDefault();
    $('body, html').animate({
        scrollTop: 0
    }, 500);
});

$(".slide > img").mousemove(function (event) {
    $(".cursor").css({ left: event.pageX, top: event.pageY });
});

$(".slide > img").hover(function (e) {
    $("body").css("cursor", "none");
    $(".cursor").css("display", "block");
}, function () {
    $("body").css("cursor", "inherit");
    $(".cursor").css("display", "none");
});


// 응찰페이지 pc화면 이미지 슬라이드
$('#galley > div').hide();
$("#galley > div").eq(0).show();

let srt = 1;
function imageslide() {
    $(".image-list ul > li").click(function(e){
        e.preventDefault();
        let idx = $(this).index();
        srt = idx;
        $(this).addClass('active').siblings().removeClass('active');
        if ($("#galley-img-copy").length > 0) {
            $("#galley-img-copy > div").eq(idx).show().siblings().hide();
        } else {
            $("#galley > div").eq(idx).show().siblings().hide();
        }
    });
}

$(window).resize(function(){
    wWidth = $(this).width();
    if(wWidth<=768){
        $(".image-list ul > li").eq(0).addClass('active').siblings().removeClass('active');
    }else{
        imageslide();
    };
});


(function () {
    var TOP = 30;
    var _scrollTop = -1;
    var box = document.querySelector('.main-content');
    var banner = document.querySelector('.main > .main-content > .right');
    var bannerH = banner.offsetHeight;
    var boxTop, boxBottom, top, bottom;

    box.style.position = 'relative';
    banner.style.position = 'absolute';
    banner.style.top = TOP + 'px';

    // 플로팅 배너 실행
    floatingBanner();

    function floatingBanner(timestamp) {
        var scrollTop = window.scrollY || window.pageYOffset;

        if (_scrollTop !== scrollTop) {
            boxTop = box.getBoundingClientRect().top + scrollTop;
            boxBottom = boxTop + box.offsetHeight;

            // 플로팅 시작
            if (scrollTop >= boxTop) {
                if (scrollTop < boxBottom - bannerH - TOP) {
                    banner.style.position = 'fixed'; // ie에서 떨림이 심해 fixed로 고정
                    top = TOP + 'px';
                    bottom = 'auto';
                    banner.style.right = 'auto';
                    /*banner.style.marginLeft = 835 + 'px';*/
                    banner.classList.add('fixed');
                } else {
                    // wrapper 바닥에 붙게
                    banner.style.position = 'absolute';
                    top = 'auto';
                    bottom = 0;
                }

            } else {
                banner.style.position = 'absolute';
                top = TOP + 'px';
                bottom = 'auto';
            }

            banner.style.top = top;
            banner.style.bottom = bottom;
            _scrollTop = scrollTop;
        }
        window.requestAnimationFrame(floatingBanner);
    }
})();


// top-header 고정 스크립트
const topHeader = document.querySelector('#top-header');
function topHeaderScroll() {
    if (pageYOffset >= 80 || pageYOffset >= 50) {
        topHeader.classList.add('topHeaderFixed');
    } else {
        topHeader.classList.remove('topHeaderFixed');
    }
}


$(window).resize(function () {
    const windowWidth = $(window).width();
    if (windowWidth <= 768){
        // top-header 고정 스크립트
        window.addEventListener('scroll', topHeaderScroll);
    }else{
        // top-header 고정 스크립트 제거
        window.removeEventListener('scroll', topHeaderScroll);
        topHeader.classList.remove('topHeaderFixed');
    }
});



const caption = Array.prototype.slice.call(document.querySelectorAll('.price-dropdown > .caption'));
caption.forEach(function (elem) {
    elem.addEventListener('click', function (e) {
        e.preventDefault();
        elem.parentNode.classList.toggle('open');
        elem.nextElementSibling.nextElementSibling.nextElementSibling.classList.toggle('block');
    });
    elem.addEventListener('focusout', function () {
        elem.nextElementSibling.nextElementSibling.nextElementSibling.classList.remove('block');
    });
});

const priceItem = Array.prototype.slice.call(document.querySelectorAll('.price-dropdown > .list > .price-item'));
priceItem.forEach(function (elem) {
    elem.addEventListener('click', function (e) {
        e.preventDefault();
        elem.parentNode.classList.remove('block');
        elem.classList.remove('selected');
        e.currentTarget.classList.add('selected');
        elem.parentNode.parentNode.classList.remove('open');
        caption.forEach(function (elem) {
            elem.innerText = e.currentTarget.innerText;
        });
    });
});



$(document).mouseup(function (e) {
    e.stopPropagation();
    var container = $(".price-dropdown");
    if (container.has(e.target).length === 0) {
        $('.list').removeClass('block');
        $('.price-dropdown').removeClass('open');
    }
});

//응찰내역 탭버튼
var bidtabBtn = $(".bidlist-btn > ul > li");
var bidtabCont = $(".bidlist-desc > div");

bidtabCont.hide().eq(0).show();
bidtabBtn.click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    var target = $(this);
    var index = target.index();
    bidtabBtn.removeClass("active");
    target.addClass("active");
    bidtabCont.css("display", "none");
    bidtabCont.eq(index).css("display", "block");
});

//응찰내역모바일 탭버튼
var bidtabBtn2 = $(".bidlistmb-btn > ul > li");
var bidtabCont2 = $(".bidlistmb-desc > div");

bidtabCont2.hide().eq(0).show();
bidtabBtn2.click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    var target = $(this);
    var index = target.index();
    bidtabBtn2.removeClass("active");
    target.addClass("active");
    bidtabCont2.css("display", "none");
    bidtabCont2.eq(index).css("display", "block");
});


// display block 함수
function displayFunc (elem) {
    elem.style.display = 'block';
}

// display none 함수
function displayNoneFunc (elem) {
    elem.style.display = 'none';
}


//버튼클릭햇을때 응찰내역 횟수 나오게하기 (pc)
$(".bid-list-cont-total2").on("click", function (e) {
    e.preventDefault();
    e.stopPropagation();
    e.stopImmediatePropagation();
    $(".bid-price .bid-list").css("display", "block");
    $('.price-choice').css("display", "none");
});

//버튼클릭햇을때 응찰내역 횟수 나오게하기 (모바일)
$(".bid-list-cont-mb-total2").on("click", function (e) {
    e.preventDefault();
    e.stopPropagation();
    e.stopImmediatePropagation();
    $("#mobilebid").css("display", "block");
    $(".mobile-bidbtn.m2 .price-choice").css("display", "none");
    document.querySelector('body').classList.add('yhidden');
});

//버튼클릭햇을때 응찰내역 전체 나오게하기 (pc)
$(".bid-price > .bid-active").on("click", function (e) {
    e.preventDefault();
    e.stopPropagation();
    $(".bid-price .bid-list").css("display", "block");
    $('.price-choice').css("display", "block");
});

//버튼클릭햇을때 임직원 응찰 경고창 나오게 하기
$(".bid-mng-alert").on("click", function (e) {
    e.preventDefault();
    if (!existBidFlag) {
        document.querySelector("#modal-popup-employees-y").classList.add('block');
        document.querySelector('.popupbg').classList.add('block');
    } else {
        $.auction.checkBid();
    }
    return false;
});

//버튼클릭햇을때 임직원 관계자 응찰 경고창 나오게 하기
$(".bid-relation_alert").on("click", function (e) {
    e.preventDefault();
    if (!existBidFlag) {
        document.querySelector("#modal-popup-employees-r").classList.add('block');
        document.querySelector('.popupbg').classList.add('block');
    } else {
        $.auction.checkBid();
    }
    return false;
});

//버튼클릭햇을때 임직원 응찰 제한 경고창 나오게 하기
$(".bid-limit-alert").on("click", function (e) {
    e.preventDefault();
    document.querySelector("#modal-popup-employees-l").classList.add('block');
    document.querySelector('.popupbg').classList.add('block');
    return false;
});


//응찰내역 닫기버튼
$("#bidlist-close").on("click", function (e) {
    e.preventDefault();
    $(".bid-list").css("display", "none");
    return false;
});


//응찰내역표 pc버전
const btnlist = $(".bidlist-desc > div.b1 > ul > li").length;
if (btnlist >= 5) {
    $(".bid-price .bid-list .bidlist-desc > div.b1").addClass("scroll")
} else if (btnlist < 5) {
    $(".bid-price .bid-list .bidlist-desc > div.b1").removeClass("scroll")
}

const btnlist2 = $(".bidlist-desc > div.b2 > ul > li").length;
if (btnlist2 >= 5) {
    $(".bid-price .bid-list .bidlist-desc > div.b2").addClass("scroll")
} else if (btnlist2 < 5) {
    $(".bid-price .bid-list .bidlist-desc > div.b2").removeClass("scroll")
}

//응찰내역표 모바일버전
const btnmblist = $(".bidlistmb-desc > div.b1 > ul > li").length;
if (btnmblist >= 5) {
    $(".bidlistmb-desc > div.b1").addClass("scroll")
} else {
    $(".bidlistmb-desc > div.b1").removeClass("scroll")
}

const btnmblist2 = $(".bidlistmb-desc > div.b2 > ul > li").length;
if (btnmblist2 >= 5) {
    $(".bidlistmb-desc > div.b2").addClass("scroll")
} else {
    $(".bidlistmb-desc > div.b2").removeClass("scroll")
}

//위시리스트
$(".heartic").click(function () {
    var el = $(this).get(0);
    if (el.dataset.wishYn === "Y") {
        if ($.wishlist.add(el.dataset.uid, 'N', false)) {
            $(".heartic-" + el.dataset.uid.toString()).find("i").attr("class", "far fa-heart");
            $(".heartic-" + el.dataset.uid.toString()).attr("data-wish-yn", "N");
        }
    } else {
        if ($.wishlist.add(el.dataset.uid, 'Y', false)) {
            $(".heartic-" + el.dataset.uid.toString()).find("i").attr("class", "fas fa-heart");
            $(".heartic-" + el.dataset.uid.toString()).attr("data-wish-yn", "Y");

            $("#wishlilst-modal").css("display", "block");
            $("body").css({ "overflow-y": "hidden", "height": "100%", "touch-action": "none" }).bind('scroll touchmove mousewheel', function (e) { e.preventDefault() });
        }
    }
});

const ctnBtn = $("#whishlist-cl");
ctnBtn.click(function (e) {
    e.preventDefault();
    $("#wishlilst-modal").css("display", "none");
});

$("#wishlilst-modal .bg").click(function (e) {
    e.preventDefault();
    $("#wishlilst-modal").css("display", "none");
});

function modalAddClass() {
    document.querySelector('body').classList.add('yhidden');
    document.querySelector('.popupbg').classList.add('block');
}

function modalRemoveClass() {
    document.querySelector('body').classList.remove('yhidden');
    document.querySelector('.popupbg').classList.remove('block');
}

// 모달창 변수
const rightMoalPopup = Array.prototype.slice.call(document.querySelectorAll('.rightmodal-modal'));

const rightmodalbtn2 = Array.prototype.slice.call(document.querySelectorAll('.rightmodalBtn2'));
rightmodalbtn2.forEach(function (element) {
    element.addEventListener('click', function (e) {
        e.preventDefault();
        const idx = Array.from(rightmodalbtn2).indexOf(e.currentTarget);
        for (i = 0; i < rightMoalPopup.length; i++) {
            rightMoalPopup[idx].classList.add('block');
            modalAddClass();
        }
    });
});

const rightmodalbtn = Array.prototype.slice.call(document.querySelectorAll('.rightmodalBtn'));
rightmodalbtn.forEach(function (element) {
    element.addEventListener('click', function (e) {
        e.preventDefault();
        const idx = Array.from(rightmodalbtn).indexOf(e.currentTarget);
        for (i = 0; i < rightMoalPopup.length; i++) {
            rightMoalPopup[idx].classList.add('block');
            modalAddClass();
        }
    });
});

document.querySelectorAll('.rightmodal-close').forEach(function (element) {
    element.addEventListener('click', function (e) {
        e.preventDefault();
        const idx = Array.from(document.querySelectorAll('.rightmodal-close')).indexOf(e.currentTarget);
        for (i = 0; i < rightMoalPopup.length; i++) {
            rightMoalPopup[idx].classList.remove('block');
            modalRemoveClass();
        }
    });
});

if (document.querySelector('.highest-bidder') !== null) {
    const highestbid = document.querySelector('.highest-bidder').cloneNode(true);
    $('.bidlistmb-btn').after(highestbid);
}

if (document.querySelector('.bidcont-desc') !== null) {
    const bidcontdesc = document.querySelector('.bidcont-desc').cloneNode(true);
    $('.mobile-bidbtn.m2 > .price-choice > .choice').after(bidcontdesc);
}

//최고 응찰가 설정 팝업
$('.bidcont-desc').click(function (e) {
    e.preventDefault();
    document.querySelector('.bid-popup.bp02').classList.add('block');
    modalAddClass();
    return false;
});

function bidpopupClose() {
    document.querySelector('.bid-popup.bp02').classList.remove('block');
    document.querySelector('.bid-popup.bp01').classList.remove('block');
    document.querySelector('.modal-bg').classList.remove('block');
    document.querySelector('.popupbg').classList.remove('block');
    if(document.body.clientWidth > 768){
        document.querySelector('body').classList.remove('yhidden');
    }else{
        document.querySelector('body').classList.add('yhidden');
    }
    return false;
}

$('.bidagree-btn > .cancel').on("click", function (e) {
    e.preventDefault();
    document.querySelector('.bid-popup.bp01').classList.remove('block');
    document.querySelector('.modal-bg').classList.remove('block');
    modalRemoveClass();
    document.querySelector("#modal-popup-employees-r").classList.remove('block');
    document.querySelector("#modal-popup-employees-y").classList.remove('block');
    document.querySelector("#modal-popup-employees-l").classList.remove('block');
    return false;
});

$('.bidbtn-bg').mouseup(function (e) {
    var f = $(".mobile-bidbtn.m2");
    if (f.has(e.target).length === 0) {
        $('#mobilebid').css("display", "none");
    }
});

const modalEmployeeBtn = Array.prototype.slice.call(document.querySelectorAll('.modal-employees-close'));
modalEmployeeBtn.forEach(function (elem) {
    elem.addEventListener("click", function (e) {
        e.preventDefault();
        $(this).parent().parent().parent().removeClass('block');
        modalRemoveClass();
        return false;
    });
});

if ($('.modal-popup-employees').is(':visible')) {
    modalAddClass();
};

function onlinebidPopup() {
    document.querySelector('.bid-popup.bp01').classList.add('block');
    modalAddClass();
    document.querySelector('#modal-popup-employees-r').classList.remove('block');
    document.querySelector('#modal-popup-employees-y').classList.remove('block');
    document.querySelector('#modal-popup-employees-l').classList.remove('block');
    return false;
};

//모바일화면일때 응찰하기버튼
$(".mobile-bidbtn.m1").on("click.pc", function (e) {
    e.preventDefault();
    if ($.loginUtils.isLogin()) {
        if (e.currentTarget.className.indexOf("address-popup") > -1) {
            // 2022.07.25 :: 모바일 
            // $.mypageUtils.openAddressPopup('I');
            $('#modal-auc-participation').modal();
        } else {
            if (aucKind === "1") {
                $.bid.redirectMajorBid(uid, event);
            } else {
                document.querySelector('#mobilebid').classList.add('block');
                document.querySelector('body').classList.add('yhidden');
                $('.mobile-bidbtn.m2').css("display", "block");
                $('.mobile-bidbtn.m2 .price-choice').css("display", "block");
            }
        }
    } else {
        $.commonUtils.openLogin(ka.msg.auction.login);
    }
    return false;
});

//모바일화면 응찰하기 닫기버튼
$("#bidlistmb-close").on("click", function (e) {
    e.preventDefault();
    document.querySelector('#mobilebid').classList.remove('block');
    document.querySelector('body').classList.remove('yhidden');
    document.querySelector('#mobilebid').style.display = "none";
    return false;
});

//4월6일수정 모바일화면 배경눌렀을때 응찰하기 닫기
$('.bidbtn-bg').mouseup(function (e) {
    var f = $(".mobile-bidbtn.m2");
    if (f.has(e.target).length === 0) {
        document.querySelector('#mobilebid').classList.remove('block');
        document.querySelector('body').classList.remove('yhidden');
    }
});

//swiper 플러그인
var swiper = new Swiper('.swiper-container', {
    pagination: {
        el: '.swiper-pagination',
    },
});

const writerInfo = Array.prototype.slice.call(document.querySelectorAll('.writer-info'));
const moreLessBtn = document.querySelector('.morelessbtn > a');
writerInfo.forEach(function (element) {
    if (element.offsetHeight >= 158) {
        element.classList.add('more');
        const moreButton = document.createElement('div');
        moreButton.className = "morelessbtn";
        moreButton.innerHTML += '<a href="javascript:void(0)">' + ka.msg.auction.descriptionOpen + ' <i class="fas fa-chevron-down"></i></a><div class="gradation"></div>';
        element.after(moreButton);
        moreButton.style.display = 'block';
    }
});

const moreLessBtnA = Array.prototype.slice.call(document.querySelectorAll('.morelessbtn > a'));
moreLessBtnA.forEach(function (btn) {
    btn.addEventListener('click', function (e) {
        e.preventDefault();
        const target = e.target;
        const location = target.parentNode.parentNode.offsetTop + window.pageYOffset / 3.5;
        const Info = target.parentNode.previousSibling;
        let isShow = target.parentNode.previousSibling.classList.contains('more');

        if (isShow) {
            e.target.innerHTML = ka.msg.auction.descriptionFold + ' <i class="fas fa-chevron-up"></i>';
            Info.classList.remove('more');
            btn.childNodes.forEach(function (element) {
                if (element.nodeName === "I") {
                    element.classList.replace('fa-chevron-down', 'fa-chevron-up');
                }
            });
            target.nextElementSibling.style.display = "none";
        } else {
            e.target.innerHTML = ka.msg.auction.descriptionOpen + ' <i class="fas fa-chevron-down"></i>';
            Info.classList.add('more');
            target.nextElementSibling.style.display = "block";
            window.scrollTo({ top: location, behavior: 'smooth', duration: 5000 });
        }
    });
});

const videoList = Array.prototype.slice.call(document.querySelectorAll('.work-info .video-info ul li > div'));
const videobutton = document.createElement('img');
videobutton.classList.add('video-playbtn');
videobutton.src = "/img/icons/work-playbtn@1x.png";
for (i = 0; i < videoList.length; i++) {
    const clone = videobutton.cloneNode();
    videoList[i].appendChild(clone);
}

function popupEmployeeY() {
    document.querySelector('#modal-popup-employees-y').classList.remove('block');
}


//추정가 여백 없애기 (pc)
const esPrice = document.querySelector('.main-content > .right > .es-price');
if(esPrice.childElementCount == 1 || esPrice.childElementCount == 2){
    esPrice.lastElementChild.style.margin = 0
}
//추정가 여백 없애기 (mobile)
const esPriceMobile = document.querySelector('.main-content > .left > .right > .es-price');
if(esPriceMobile.childElementCount == 1 || esPriceMobile.childElementCount == 2){
    esPriceMobile.lastElementChild.style.margin = 0
}


// 드롭다운메뉴 스크립트 임시주석처리
const lotDropdown = document.querySelector('.lot-dropdown');
const lotBtn = document.querySelector('.top-lot-num');

document.querySelector('.lot-btn').addEventListener('submit', function(e){
    e.preventDefault();
});


// 모바일 mobile-top안에 경매리스트아이콘 넣기
const topListIcon = document.querySelector('.top-list').cloneNode(true);
document.querySelector('.mobile-top').prepend(topListIcon);


let work_address_tooltip = document.createElement('div');
work_address_tooltip.className = "work_address_tooltip";
work_address_tooltip.innerHTML = '<i class="fas fa-times"></i>' + ka.msg.auction.regReqInfoForBid;

if (document.querySelectorAll('.bid-address-btn') && document.querySelectorAll('.bid-address-btn').length > 0) {
    document.querySelectorAll('.bid-address-btn')[0].append(work_address_tooltip);
}

if (document.querySelector(".work_address_tooltip > i")) {
    document.querySelector(".work_address_tooltip > i").addEventListener('click', function (e) {
        e.preventDefault();
        e.currentTarget.parentNode.style.display = "none";
    });
}


const canvasfunc = function (type, canvasObj, imgSrc, sizeWidth, sizeLength) {

    var offset = { x: 0, y: 0 };
    var canvas = canvasObj;
    var context = canvas.getContext("2d");
    var currentX = canvas.width/2;
    var currentY = canvas.height/2;
    var star_img = new Image();

    star_img.src = imgSrc;
    star_img.onload = function() {
        context.clearRect(0, 0, canvas.width, canvas.height);
        context.beginPath();
        _DrawImage(sizeWidth, sizeLength);
        mouseUp();
    };
    var cWidth = null;

    if (type === "default_mode") {
        cWidth = sizeWidth / 350;
    } else if (type === "close_mode") {
        cWidth = sizeWidth / 150;
    }


    var heightC = star_img.naturalHeight / star_img.naturalWidth;
    var starWidth = 998 * cWidth;
    var starHeight = (998 * cWidth) *  heightC;
  

    canvas.addEventListener('mousedown', mouseDown, false);
    canvas.addEventListener('mouseup', mouseUp, false);
    window.addEventListener('mouseup', mouseUp, false);
    
    function mouseUp(){
        window.removeEventListener('mousemove', popupMove, true);
    }

    function mouseDown(e){
        offset.x = e.clientX - canvas.offsetLeft;
        offset.y = e.clientY - canvas.offsetTop;
        window.addEventListener('mousemove', popupMove, true);
    }

    function popupMove(e){
        canvas.style.position = 'absolute';
        var top = e.clientY - offset.y;
        var left = e.clientX - offset.x;
        canvas.style.top = top + 'px';
        canvas.style.left = left + 'px';
    }


    function _DrawImage(sizeWidth, sizeLength) {
        context.shadowColor = "rgba(0,0,0,0.3)";
        context.shadowBlur = 2.5;
        context.shadowOffsetX = 1.5;
        context.shadowOffsetY = 1.5;
        if (Number(sizeWidth) <= 75 && Number(sizeLength) <= 75) {
            context.drawImage(star_img, currentX-(starWidth/2), currentY-(starHeight/1.8), starWidth, starHeight);
        } else {
            context.drawImage(star_img, currentX-(starWidth/2), currentY-(starHeight/1.5), starWidth, starHeight);
        }

    }


};




