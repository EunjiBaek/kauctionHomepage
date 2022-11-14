export class TableBody {
    constructor(tableId) {
        this.$tableBody = document.querySelector(`#${tableId}`);
        // TODO(lth): table body empty 구조를 <tbody /> 태그 내에서 관리 가능 하도록 구조 개선 필요
        this.$tableBodyEmpty = document.querySelector(`#${tableId}-empty`);
    }

    clear = () => {
        this.$tableBody.innerHTML = '';
        this.$tableBodyEmpty.style.display = 'none';
    };

    showEmpty = () => {
        this.$tableBodyEmpty.style.display = 'block';
    };
}
