export class ContentTab {
    static CONTENT_ID = 'data-content-id';

    constructor(listId) {
        this.$list = document.querySelector(`#${listId}`);
        this.$content = document.querySelector(`#${this.$list.getAttribute(ContentTab.CONTENT_ID)}`);
    }

    setEvent = (pathname) => {
        this.$list.addEventListener('click', () => {
            location.href = pathname;
        });
    };

    selectTab = () => {
        const $originLi = this.$list.parentElement.querySelector('.active');
        if ($originLi) {
            $originLi.classList.remove('active');
            document.querySelector(`#${$originLi.getAttribute(ContentTab.CONTENT_ID)}`).style.display = 'none';
        }

        this.$list.classList.add('active');
        this.$content.style.display = 'block';
    };
}
