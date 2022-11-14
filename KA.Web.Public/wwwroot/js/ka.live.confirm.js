
(function ($) {	
	var pluginName = "layerConfirm";
	var pluginDataName = "plugin_" + pluginName;

	function Plugin(element, options) {
		this.element = element;

		this._attachedEvents = [];

		this.params = {
			titleHtml: "<h1>컨펌</h1>"
			, messageHtml: "진행하시겠습니까?"
			, okText: "확인"
			, cancelText: "취소"			
			, completed: null			
			, confirmed: null
		};
		this.params = $.fn.extend(this.params, options);

		this.init();
	}
	Plugin.prototype = {
		init: function () {
			this.initTemplate();
			this.initButtons();
			if (this.$lcElement != undefined) {
				this._attachEvent(this.$lcElement, "click", this._onBackgroundClick.bind(this));
			}
		},
		initTemplate: function () {			

			var btnTemplate = "";
			if (this.params.cancelText.length > 0 || this.params.okText.length > 0) {
				btnTemplate += '<div class="btn">';
				if (this.params.cancelText.length > 0)
					btnTemplate += '<a href="#" class="cancel">{cancelText}</a>'.replace("{cancelText}", this.params.cancelText);
				if (this.params.okText.length > 0)
					btnTemplate += '<a href="#" class="check">{okText}</a>'.replace("{okText}", this.params.okText);

				btnTemplate += '</div>';	
			}

			var template = '<div class="modal p5" id="layerConfirm"><div class="modal-bg"><div class="modal-cont">'
				+ this.params.titleHtml
				+ this.params.messageHtml
				+ btnTemplate
				+ '<a href="#" class="close"><span></span><span></span></a>'
				+ '</div></div></div>'
				;		

			if ($('body').find("#layerConfirm").length <= 0) {
				$('body').append(template);
				this.$lcElement = $('body').find("#layerConfirm");
				//$(".confirmMsg").eq(0).focus();				
				this.$lcElement.show();
			}
		},
		initButtons: function () {
			if (this.$lcElement != undefined) {

				//var click = (('ontouchstart' in window)) ? 'touchstart' : 'click';
				var click = "click";
				this._attachEvent(this.$lcElement.find('.close')		, click, this._onCancelClick.bind(this));
				this._attachEvent(this.$lcElement.find('.cancel')		, click, this._onCancelClick.bind(this));
				this._attachEvent(this.$lcElement.find('.check')		, click, this._onOKClick.bind(this));
				this._attachEvent(this.$lcElement.find('.modal-cont')	, click, function(e) { e.stopPropagation();	e.stopPropagation();});
			}
		},
		_attachEvent: function (el, ev, fn) {
			el.on(ev, null, null, fn);
			this._attachedEvents.push([el, ev, fn]);
		},
		_onOKClick: function (e) {			
			e.preventDefault();
			e.stopPropagation();

			this.$lcElement.blur();						
			if (this.params.confirmed != null) {
				if (this.params.confirmed.apply(null, [])) {					
					this.$lcElement.remove();
					this.Select(true);
                }
			} else {
				this.$lcElement.remove();
				this.Select(true);
            }
		},
		_onCancelClick: function () {
			this.$lcElement.blur();			
			this.$lcElement.remove();
			this.Select(false);
		},
		_onBackgroundClick: function (e) {
			e.preventDefault();
			e.stopPropagation();
			this.$lcElement.remove();
			this.Select(false);
		},
		Select: function (result) {
			this.params.completed.apply(null, [result]);            
			return this.params;
		}
	}

	$[pluginName] = function (options, p) {
		new Plugin(this, options)
		return this;
	};
})(jQuery);