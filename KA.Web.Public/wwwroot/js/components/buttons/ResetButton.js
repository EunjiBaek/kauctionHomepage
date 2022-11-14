export class ResetButton {
    constructor(btnId) {
        this.$btn = document.querySelector(`#${btnId}`);
    }

    setEvent(dataCallback, param, ...args) {
        this.$btn.addEventListener('click', async () => {
            args.forEach((arg) => arg());
            await dataCallback(param);
        });
    }
}