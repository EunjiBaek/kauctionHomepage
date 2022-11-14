export class SingleCheckboxGroup {
    constructor(divId, eventCallback) {
        this.$div = document.querySelector(`#${divId}`);
        this.$checkBoxes = this.$div.querySelectorAll('input[type="checkbox"]');
        this.eventCallback = eventCallback;
        this.init();
    }

    init = () => {
        this.$checkBoxes.forEach((checkBox) => {
            checkBox.checked = false;
        })
    }
    
    setEvent = (parameters) => {
        this.$checkBoxes.forEach((targetItem) => {
            targetItem.addEventListener('click', async () => {
                this.$checkBoxes.forEach((otherItem) => {
                    if (targetItem !== otherItem) {
                        otherItem.checked = false;
                    }
                });
                await this.eventCallback(parameters);
            });
        });
    };
    
    getCheckedValue = () => {
        return this.$div.querySelector('input:checked')?.value;
    };
}
