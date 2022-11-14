export class MultipleCheckboxGroup {
    constructor(checkboxName, getDataCallback) {
        this.$checkBoxes = document.querySelectorAll(`input[name="${checkboxName}"]`);
        this.getDataCallback = getDataCallback;
        this.init();
    }
    
    init = () => {
        this.$checkBoxes.forEach((checkBox) => {
            checkBox.checked = false;
        })
    }

    setEvent = (parameters) => {
        this.$checkBoxes.forEach((checkBox) => {
            checkBox.addEventListener('click', async () => {
                await this.getDataCallback(parameters);
            });
        });
    };

    getCheckedValues = () => {
        const values = [];
        this.$checkBoxes.forEach((checkBox) => {
            if (checkBox.checked) {
                values.push(checkBox.value);
            }
        });
        return values;
    };
}