export class Modal {
    static CLOSE_MODAL_CLASS_NAME = 'close-modal';

    constructor(modalId) {
        this.modalId = modalId;
        this.$modal = document.querySelector(`#${modalId}`);
    }

    /**
     * reset()
     * modal dom 초기화(이벤트 리스너 초기화)
     */
    reset() {
        const newNode = this.$modal.cloneNode(true);
        this.$modal.parentNode.insertBefore(newNode, this.$modal);
        this.$modal.parentNode.removeChild(this.$modal);
        this.$modal = document.querySelector(`#${this.modalId}`);
        this.#setCloseEvents();
    }

    show() {
        this.$modal.style.display = 'block';
        document.body.classList.add('scroll_lock');
    }

    close() {
        this.$modal.style.display = 'none';
        document.body.classList.remove('scroll_lock');

        if (document.body.clientWidth > 768) {
            document.body.classList.remove('scroll_lock');
        }
    }

    #setCloseEvents() {
        this.$modal.querySelectorAll(`.${Modal.CLOSE_MODAL_CLASS_NAME}`).forEach((close) => {
            close.addEventListener('click', () => {
                this.close();
            });
        });

        this.$modal.addEventListener('click', (e) => {
            if (this.$modal.style.display === 'block') {
                if (e.target === this.$modal || e.target === this.$modal.firstElementChild) {
                    this.close();
                }
            }
        });

    }
}
