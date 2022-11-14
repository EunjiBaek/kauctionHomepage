export class SearchInput {
    constructor(inputId, btnId, getDataCallback) {
        this.$input = document.querySelector(`#${inputId}`);
        this.$btn = document.querySelector(`#${btnId}`);
        this.getDataCallback = getDataCallback;
        this.init();
    }
    
    init = () => {
        this.$input.value = '';
    }
    
    setEvent = (parameters) => {
        this.$input.addEventListener('keydown', async (event) => {
            const key = event.key || event.keyCode;
            if (key === 'Enter' || key === 13) {
                await this.getDataCallback(parameters);
            }
        });

        this.$btn.addEventListener('click', async () => {
            await this.getDataCallback(parameters);
        });
    };

    getSearchText = () => {
        return this.$input.value;
    };
}
