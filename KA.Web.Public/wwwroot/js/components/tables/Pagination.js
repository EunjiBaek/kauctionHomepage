export class Pagination {
    constructor(paginationId, callback) {
        this.$pagination = document.querySelector(`#${paginationId}`);
        this.callback = callback;
    }

    render = (parameters) => {
        const option = {
            $target: this.$pagination,
            totalCount: null,
            pageSize: 10,
            pageBlock: 10,
            page: 1,
            callback: this.callback,
            parameters: parameters,
            moveTop: true,
        };

        if (typeof isMobile === 'string' && isMobile === 'True') {
            option.pageBlock = 5;
        }

        Object.assign(option, parameters);

        const totalPage = option.totalCount % option.pageSize !== 0 ? parseInt(option.totalCount / option.pageSize, 10) + 1 : parseInt(option.totalCount / option.pageSize, 10);
        const startPage =
            option.page % option.pageBlock !== 0
                ? parseInt(option.page / option.pageBlock, 10) * option.pageBlock + 1
                : parseInt(option.page / option.pageBlock - 1, 10) * option.pageBlock + 1;
        const endPage = startPage + option.pageBlock - 1 < totalPage ? startPage + option.pageBlock - 1 : totalPage;

        if (option.$target !== null && !isNaN(totalPage)) {
            this.clear();

            let $li;
            $li = this.createPageElement(option, 1, false, ['first', option.page === 1 ? 'disabled' : '_'], ['fa-chevron-double-left']);
            option.$target.append($li);

            $li = this.createPageElement(option, startPage - 1, false, ['previous', option.page <= option.pageBlock ? 'disabled' : '_'], ['fa-chevron-left']);
            option.$target.append($li);

            for (let i = startPage; i <= endPage; i++) {
                $li = this.createPageElement(option, i, true, [option.page === i ? 'active' : '_'], [], function (target, parentElement) {
                    target.querySelector('.active').classList.remove('active');
                    parentElement.classList.add('active');
                });
                option.$target.append($li);
            }

            $li = this.createPageElement(option, endPage + 1, false, ['next', endPage >= totalPage ? 'disabled' : '.'], ['fa-chevron-right']);
            option.$target.append($li);

            $li = this.createPageElement(option, totalPage, false, ['last', option.page === totalPage ? 'disabled' : '.'], ['fa-chevron-double-right']);
            option.$target.append($li);
        }
    };

    createPageElement(option, indexPage, isNumberBtn, listClasses, linkClasses, clickCallback) {
        const $li = document.createElement('li');
        $li.classList.add('paginate_button', 'page-item', ...listClasses);

        const $a = document.createElement('a');
        $a.classList.add('page-link');
        if (!listClasses.includes('disabled')) {
            $a.addEventListener('click', function () {
                option.callback({
                    page: indexPage,
                });

                if (listClasses.includes('active')) {
                    clickCallback(option.$target, $li);
                }
            });
        }

        if (isNumberBtn) {
            $a.innerText = indexPage.toString();
        } else {
            const $i = document.createElement('i');
            $i.classList.add('far', 'fa-fw', 'fs-9', ...linkClasses);
            $a.append($i);
        }

        $li.append($a);
        return $li;
    }

    clear = () => {
        this.$pagination.innerHTML = '';
    };
}
