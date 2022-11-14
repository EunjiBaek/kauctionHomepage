export class LoadingContent {
    constructor($btn, loadingMessage = 'Loading') {
        this.$btn = $btn;
        this.loadingMessage = loadingMessage;
        this.$tempBtnContent = undefined;
    }
    
    start() {
        this.$btn.setAttribute('disabled', true);
        this.$tempBtnContent = this.$btn.innerHTML;
        this.$btn.innerHTML = `
            <i class='fa fa-spinner fa-spin'></i>
            ${this.loadingMessage}
        `;
    }
    
    end() {
        this.$btn.removeAttribute('disabled');
        if (this.$tempBtnContent) {
            this.$btn.innerHTML = this.$tempBtnContent;
        }
    }
}