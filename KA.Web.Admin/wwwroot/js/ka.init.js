(function ($) {
    'use strict';

    $(document).ready(function () {
        $(".list-view-wrapper").scrollbar();

        $('.modal').on('show.bs.modal', function (e) {
            if ($(e.currentTarget).attr("data-popup")) {
                $("body").addClass("body-scrollable");
            }
        });

        $('.modal').on('hidden.bs.modal', function (e) {
            $("body").removeClass("body-scrollable");
            if ($(this).find('form').length > 0) {
                $(this).find('form')[0].reset();
            }
        });

        $('.datepicker').datepicker({
            format: 'yyyy-mm-dd',
            autoclose: true,
            language: "ko"
        });

        $('.required > label').append(
            "<span style='color: red;'>&nbsp;*</span>"
        );
    });

})(window.jQuery);