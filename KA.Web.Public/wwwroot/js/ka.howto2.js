//howto 버튼 고정 스크립트
const howtoffset = $("#Howto").offset().top*0.89;
$(window).scroll(function(){
const wScroll = $(this).scrollTop();
    if (matchMedia("screen and (max-width: 1275px)").matches){
        if(wScroll > howtoffset){
            $(".howtobtn").addClass('pos');
            $("#Howto > .howto-desc").css({"margin-top":"95px"});
        }else{
            $(".howtobtn").removeClass('pos');
            $("#Howto > .howto-desc").css({"margin-top":"0px"});
        }
    } else {
        $(".howtobtn").removeClass('pos');
        $("#Howto > .howto-desc").css({"margin-top":"0px"});
    }
});


//위탁절차 탭메뉴
const cosproceBtn = $(".cosproce-btn > ul > li");
const cosproceCont = $(".cosproce-desc > div");      
cosproceCont.hide().eq(0).show();
cosproceBtn.click(function(e){
    e.preventDefault();
    const target = $(this);
    const index = target.index();
    cosproceBtn.removeClass("active");
    target.addClass("active");
    cosproceCont.css("display","none");
    cosproceCont.eq(index).css("display","block");
});
