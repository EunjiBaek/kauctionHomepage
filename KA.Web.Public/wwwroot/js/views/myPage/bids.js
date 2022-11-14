import { Localizer } from '../../utils/Localizer.js';
import { SearchInput } from '../../components/forms/SearchInput.js';
import { Pagination } from '../../components/tables/Pagination.js';
import { ContentTab } from '../../components/buttons/ContentTab.js';
import { TableBody } from '../../components/tables/TableBody.js';
import { Modal } from '../../components/modals/Modal.js';
import { DatePicker } from '../../components/forms/DatePicker.js';
import { EAuctionKind } from '../../types/enums.js';
import { DeviceDetector } from '../../utils/DeviceDetector.js';
import { SingleCheckboxGroup } from '../../components/forms/SingleCheckboxGroup.js';
import { ImagePathProcessor } from '../../utils/ImagePathProcessor.js';
import { HtmlUtils } from '../../utils/HtmlUtils.js';
import { MultipleCheckboxGroup } from '../../components/forms/MultipleCheckboxGroup.js';
import { RouteValidator } from '../../utils/RouteValidator.js';
import { ResetButton } from '../../components/buttons/ResetButton.js';

$(async function () {
    'use strict';

    const bidDatePicker = new DatePicker('bid-date-picker', 1);
    const successfulBidDatePicker = new DatePicker('successful-bid-date-picker', 1);

    const bidTab = new ContentTab('bid-li');
    const successfulBidTab = new ContentTab('successful-bid-li');

    bidTab.setEvent('/myPage/bids');
    successfulBidTab.setEvent('/myPage/successfulBids');

    const bidsKindCheckboxes = new MultipleCheckboxGroup('bid_state_check', getMyBids);
    const successfulBidsKindCheckboxes = new MultipleCheckboxGroup('successfulBid_check', getMySuccessfulBids);

    bidsKindCheckboxes.setEvent({ page: 1 });
    successfulBidsKindCheckboxes.setEvent({ page: 1 });

    const bidStateCheckboxGroup = new SingleCheckboxGroup('auc-state-checkbox-group', getMyBids);
    bidStateCheckboxGroup.setEvent({ page: 1 });

    const bidSearchInput = new SearchInput('search-bid-input', 'search-bid-btn', getMyBids);
    const successfulBidSearchInput = new SearchInput('search-successful-bid-input', 'search-successful-bid-btn', getMySuccessfulBids);
    bidSearchInput.setEvent({ page: 1 });
    successfulBidSearchInput.setEvent({ page: 1 });

    const bidResetBtn = new ResetButton('bid-reset-btn');
    bidResetBtn.setEvent(getMyBids, { page: 1 }, bidDatePicker.init, bidsKindCheckboxes.init, bidStateCheckboxGroup.init, bidSearchInput.init);

    const successfulBidResetBtn = new ResetButton('successful-bid-reset-btn');
    successfulBidResetBtn.setEvent(getMySuccessfulBids, { page: 1 }, successfulBidDatePicker.init, successfulBidsKindCheckboxes.init, successfulBidSearchInput.init);

    if (RouteValidator.lastSegmentStartsWith('bids')) {
        bidTab.selectTab();
        await getMyBids({ page: 1 });
    } else if (RouteValidator.lastSegmentStartsWith('successfulBids')) {
        successfulBidTab.selectTab();
        await getMySuccessfulBids({ page: 1 });
    } else {
        console.error('잘못된 경로 입니다.');
        bidTab.selectTab();
        await getMyBids({ page: 1 });
    }

    const bidDetailModal = new BidDetailModal('bid-detail-modal', 'bid-detail-content');
    const bidTableBody = new BidTableBody('bid-table-body', bidDetailModal.show);
    const bidPagination = new Pagination('bid-pagination', getMyBids);

    const bidNotificationModal = new BidNotificationModal('modal-bid-noti');
    const guaranteeModal = new GuaranteeModal('guarantee-modal', 'guarantee-btn', 'guarantee-print-checkbox', getMySuccessfulBids);
    const successfulBidTableBody = new SuccessfulBidTableBody('successful-bid-table-body', bidNotificationModal.show, guaranteeModal.show);
    const successfulBidPagination = new Pagination('successful-bid-pagination', getMySuccessfulBids);

    async function getMyBids(parameters) {
        const [startDate, endDate] = bidDatePicker.getSearchDateRange();
        const apiParameters = {
            start_date: startDate,
            end_date: endDate,
            auc_kinds: bidsKindCheckboxes.getCheckedValues(),
            nak_yn: bidStateCheckboxGroup.getCheckedValue(),
            search: bidSearchInput.getSearchText(),
            ...parameters,
        };

        await $.ajaxUtils.getApiData('/api/MyPage/GetBidList', apiParameters, getMyBidsComplete);
    }

    function getMyBidsComplete(result, parameters) {
        if ($.ajaxUtils.getResultCode(result) !== '00') {
            $.commonUtils.alert(ka.msg.common.error);
        }

        parameters = JSON.parse(parameters);
        const totalCount = result.data[0]?.total_count ?? 0;

        bidTableBody.clear();
        result.data.forEach((item) => {
            bidTableBody.renderRow(item);
        });

        if (result.data.length === 0) {
            bidTableBody.showEmpty();
        }

        if (totalCount > 0) {
            bidPagination.render({
                totalCount,
                ...parameters,
            });
        } else {
            bidPagination.clear();
        }
    }

    async function getMySuccessfulBids(parameters) {
        const [startDate, endDate] = successfulBidDatePicker.getSearchDateRange();
        const apiParameters = {
            start_date: startDate,
            end_date: endDate,
            auc_kinds: successfulBidsKindCheckboxes.getCheckedValues(),
            search: successfulBidSearchInput.getSearchText(),
            ...parameters,
        };

        await $.ajaxUtils.getApiData('/api/MyPage/GetSuccessfulBidList', apiParameters, getMySuccessfulBidsComplete);
    }

    function getMySuccessfulBidsComplete(result, parameters) {
        if ($.ajaxUtils.getResultCode(result) !== '00') {
            $.commonUtils.alert(ka.msg.common.error);
        }

        const passThoroughParam = JSON.parse(parameters);

        const totalCount = result.data[0]?.total_count ?? 0;

        successfulBidTableBody.clear();
        result.data.forEach((item) => {
            successfulBidTableBody.renderRow(item);
        });

        if (result.data.length === 0) {
            successfulBidTableBody.showEmpty();
        }

        if (totalCount > 0) {
            successfulBidPagination.render({
                totalCount,
                ...passThoroughParam,
            });
        } else {
            successfulBidPagination.clear();
        }
    }
});

export class BidTableBody extends TableBody {
    constructor(tableId, showDetailCallback) {
        super(tableId);
        this.showDetailCallback = showDetailCallback;
        this.imagePathProcessor = new ImagePathProcessor();
    }

    renderRow = (item) => {
        const $tr = document.createElement('tr');
        let aucKind = '';
        switch (item.auc_kind) {
            case EAuctionKind.majorAbsentee:
                aucKind = ka.msg.mypage.majorAbsentee;
                break;
            case EAuctionKind.majorLive:
                aucKind = ka.msg.mypage.majorLive;
                break;
            case EAuctionKind.premium:
                aucKind = ka.msg.mypage.premium;
                break;
            case EAuctionKind.weekly:
                aucKind = ka.msg.mypage.weekly;
                break;
            default:
                console.warn(ka.msg.mypage.aucKindErrorMsg);
                break;
        }

        $tr.innerHTML = `
            <td>${aucKind}</td>
            <td>${item.auc_start_date.split(' ')[0]}</td>
            <td class="work_info">
                <div>
                    <div class="img">
                        <img
                            loading="lazy"
                            onerror="this.src='/img/list_noimg.jpg'"
                            src="${this.imagePathProcessor.processAuctionImage(item.img_file_name, item.auc_kind, item.auc_num, false)}"
                            alt="">
                    </div>
                    <div class="work_detail">
                        <strong>LOT${item.lot_num}. ${item.artist_name}</strong>
                        <span>${item.title}</span>
                    </div>
                </div>
            </td>
            <td>
                <div id="show-bid-detail-btn" class="bid_detail_wrap">
                    ${item.bid_cnt} ${ka.msg.mypage.cases} <span style="display: inline-block; font-sizw: 18px; padding-left: 3px;">+</span>
                </div>
            </td>
            <td>${item.bid_reg_date}</td>
            <td class="price">${$.stringUtils.comma(item.price_bid)}</td>
            <td>${item.nak_yn === 'Y' ? ka.msg.auction.winningBid : ka.msg.auction.bid}</td>
        `;

        const $showBidDetailBtn = $tr.querySelector('#show-bid-detail-btn');
        $showBidDetailBtn.addEventListener('click', () => {
            this.showDetailCallback(item);
        });

        this.$tableBody.append($tr);
    };
}

export class SuccessfulBidTableBody extends TableBody {
    constructor(tableId, showBidNotificationModalCallback, showPrintModalCallback) {
        super(tableId);
        this.showBidNotificationModalCallback = showBidNotificationModalCallback;
        this.showPrintModalCallback = showPrintModalCallback;
        this.imagePathProcessor = new ImagePathProcessor();
    }

    renderRow = (item) => {
        const $tr = document.createElement('tr');
        let aucText = '';
        switch (item.auc_kind) {
            case EAuctionKind.majorAbsentee:
                aucText = ka.msg.mypage.major;
                break;
            case EAuctionKind.premium:
                aucText = ka.msg.mypage.premium;
                break;
            case EAuctionKind.weekly:
                aucText = ka.msg.mypage.weekly;
                break;
            default:
                console.warn(ka.msg.mypage.aucKindErrorMsg);
                break;
        }
        const feePrice = Number(item.price_bid) * parseFloat(item.fees);

        const bidRegDateArr = item.bid_reg_date.split(" ");


        $tr.innerHTML = `
            <td>${aucText}</td>
            <td>${item.auc_start_date.split(' ')[0]}</td>
            <td class="work_info">
                <div>
                    <div class="img">
                        <img
                            loading="lazy"
                            onerror="this.src='/img/list_noimg.jpg'"
                            src="${this.imagePathProcessor.processAuctionImage(item.img_file_name, item.auc_kind, item.auc_num, false)}"
                            alt="">
                    </div>
                    <div class="work_detail">
                        <strong>LOT${item.lot_num}. ${item.artist_name}</strong>
                        <span>${item.title}</span>
                    </div>
                </div>
            </td>
            <td>
                ${bidRegDateArr[0]} <br />
                ${bidRegDateArr[1]}
            </td>

            <td class="price">${$.stringUtils.comma(item.price_bid)}</td>
            <td class="price">${$.stringUtils.comma(feePrice.toString())}</td>
            <td id="successful-bid-actions">
                <div>
                    <a id="winning-big-result-btn">
                        ${ka.msg.list.viewWinninBidResult}
                    </a>
                </div>
            </td>

            <td id="certificate-warranty"></td>
        `;

        const winningBidResultBtn = $tr.querySelector('#winning-big-result-btn');
        winningBidResultBtn.addEventListener('click', () => {
            this.showBidNotificationModalCallback(item.auc_kind, item.auc_num);
        });

        const certBtn = CertificateButton.createOrNull(item, this.showPrintModalCallback);
        if (certBtn) {
            const $td = $tr.querySelector('#certificate-warranty');
            $td.append(certBtn);
        }

        this.$tableBody.append($tr);
    };
}

export class CertificateButton {
    static createOrNull(item, showPrintModalCallback) {
        let $certButton = null;
        // 보증서 있는 경우
        if (item.certificate_yn === 'Y') {
            // 보증서 조건을 우선으로 처리
            if (item.certificate_delivery_state === '001') {
                $certButton = HtmlUtils.toElement(`
                    <div class="certificate-warranty-text">
                        <span>${ka.msg.mypage.certificateReqDate}</span>
                        <span>${item.certificate_delivery_reg_date}</span>
                    </div>
                `);
                $certButton.addEventListener('click', () => {
                    CertificateButton.#certificateAlert('R');
                });
            } else {
                if (item.certificate_print_yn !== 'Y') {
                    $certButton = HtmlUtils.toElement(`
                        <button type="button" class="certificate-warranty-btn btn_blue_bd_nt">
                            ${ka.msg.successfulBid.certificate}
                        </button>
                    `);
                    $certButton.addEventListener('click', () => {
                        showPrintModalCallback(item.ow_uid, item.certificate_yn);
                    });
                }
            }
        } else {
            // 이미 보증서가 출력된 경우: 케이옥션 홈페이지 혹은 케이오피스 통해서 기 출력 가능
            if (item.certificate_print_yn === 'Y') {
                $certButton = HtmlUtils.toElement(`
                    <div class="certificate-warranty-text">
                        <span>${ka.msg.mypage.certificatePrintDate}</span>
                        <span>${item.certificate_print_date}</span>
                    </div>
                `);
                $certButton.addEventListener('click', () => {
                    CertificateButton.#certificateAlert('C');
                });
            }
        }

        return $certButton;
    }

    static #certificateAlert = (type) => {
        let messageContent = undefined;
        if (type === 'C') {
            messageContent = `<span style="font-weight: bold;">${ka.msg.mypage.certificateComplete1}</span><br /><br />${ka.msg.mypage.certificateComplete2}<br />${ka.msg.mypage.certificateComplete3}`;
        } else if (type === 'R') {
            messageContent = `<span style="font-weight: bold;">${ka.msg.mypage.certificateRequest1}</span><br /><br />${ka.msg.mypage.certificateRequest2}<br />${ka.msg.mypage.certificateRequest3}`;
        }
        $.commonUtils.alert(messageContent, 'success');
    };
}

export class BidDetailModal extends Modal {
    static CONTENT_DIV_SELECTOR = '.cnt';
    static BUTTON_SELECTOR = '.modal_table_btn_wrap > button';
    constructor(modalId, contentDivId) {
        super(modalId);
        this.contentDivId = contentDivId;
        this.imagePathProcessor = new ImagePathProcessor();
    }
    
    reset() {
        super.reset();
        this.$contentDiv = this.$modal.querySelector(BidDetailModal.CONTENT_DIV_SELECTOR);
        this.$button = this.$modal.querySelector(BidDetailModal.BUTTON_SELECTOR);
        this.bidDetailContent = new BidDetailContent(this.contentDivId);
    }

    show = async (item) => {
        this.reset();
        this.$button.style.display = 'block';
        this.bidDetailContent.init();

        const $aucDate = this.$modal.querySelector('#auc-date');
        $aucDate.textContent = item.auc_start_date.split(' ')[0];
        this.$modal.querySelector('#work-img').setAttribute('src', this.imagePathProcessor.processAuctionImage(item.img_file_name, item.auc_kind, item.auc_num, false));
        const $workInfo = this.$modal.querySelector('#work-info');
        $workInfo.textContent = `LOT${item.lot_num}. ${item.artist_name}`;

        const $workTitle = this.$modal.querySelector('#work-title');
        $workTitle.textContent = item.title;

        const param = { work_seq: item.uid, lot_num: item.lot_num };
        const result = await BidDetailModal.#getMyWorkBids(param);

        if ($.ajaxUtils.getResultCode(result) !== '00') {
            $.commonUtils.alert(ka.msg.common.error);
        }

        result.data.forEach((workBid) => {
            Object.assign(workBid, param);
            this.bidDetailContent.render(workBid);
        });

        this.$contentDiv.firstElementChild.style.display = 'block';
        super.show();
    };

    static #getMyWorkBids(parameters) {
        return $.ajaxUtils.getApiData(`/api/Auction/MyBidList/${parameters.work_seq}`, parameters, null, false);
    }
}

export class BidDetailContent {
    constructor(contentDivId) {
        this.$contentDiv = document.querySelector(`#${contentDivId}`);
    }
    
    init() {
        this.$contentDiv.innerHTML = '';
    }
    
    render = (item) => {
        const $li = document.createElement('li');
        $li.innerHTML = `
            <div>•</div>
            <p>
                    LOT${item.lot_num}. ${ka.msg.auction.bid} ${Number(item.my_count) - this.$contentDiv.childElementCount} ${ka.msg.mypage.counts}. ${item.reg_ymd} ${item.reg_hms} (${ka.msg.mypage.successBid}${
            item.nak_yn === 'Y' ? `, ${ka.msg.auction.winningBid}` : ''})
            </p>
        `;
        this.$contentDiv.appendChild($li);
    };
}

export class BidNotificationModal extends Modal {
    static TABLE_DIV_CLASS = 'modal-bid-noti-table';
    static BUTTON_ID = 'bid-notification-modal-btn';

    constructor(modalId) {
        super(modalId);
        this.$tableDiv = this.$modal.querySelector(`.${BidNotificationModal.TABLE_DIV_CLASS}`);
        this.$button = this.$modal.querySelector(`#${BidNotificationModal.BUTTON_ID}`);
        this.isKor = Localizer.getLanguage() === 'ko-KR';
    }

    show = async (aucKind, aucNum) => {
        const result = await BidNotificationModal.#getBidNotification(aucKind, aucNum);
        if (result?.data?.mail_send_yn !== 'Y') {
            $.commonUtils.alert(ka.msg.successfulBid.notYetAlert);
            return;
        }

        this.#renderTable(result.data);

        this.$modal.querySelector('#auc_title').textContent = result.data.auc_title;
        this.$modal.querySelector('#expire_date').textContent = result.data.expire_date;

        this.$button.addEventListener('click', function () {
            window.location.href = `/MyPage/SuccessfulBidDocument/${aucKind}/${aucNum}`;
        });

        super.show();
    };

    #renderTable(data) {
        const colspan = this.isKor ? '5' : '4';
        const rowspan = this.isKor ? '2' : '1';

        this.$tableDiv.innerHTML = '';

        const $headerTable = document.createElement('table');
        $headerTable.className = 'bidnoti-table';
        $headerTable.style.width = '100%';
        $headerTable.style.borderCollapse = 'collapse';
        $headerTable.style.fontFamily = '"Noto Sans", sans-serif';
        $headerTable.style.fontSize = '15px';
        $headerTable.style.textAlign = 'right';
        $headerTable.style.borderTop = '1px solid #ccc';
        $headerTable.style.borderBottom = '1px solid #ccc';
        $headerTable.cellspacing = '0';
        $headerTable.cellpadding = '4';
        $headerTable.id = 'auction_nak';

        $headerTable.innerHTML = `
            <tr style="background-color:#F5F5F5; text-align: center;">
                <th rowspan="${rowspan}" style="padding:4px; border: 1px solid #ccc;">
                    <strong style="font-size: 12px;">Lot</strong>
                </th>
                <th rowspan="${rowspan}" style="padding:4px; border: 1px solid #ccc;">
                    <strong style="font-size: 12px;">${ka.msg.successfulBid.title}</strong>
                </th>
                <th rowspan="${rowspan}" style="padding:4px; border: 1px solid #ccc;">
                    <strong style="font-size: 12px;">${ka.msg.auction.hammerPrice}<br/>(A)</strong>
                </th>
                <th rowspan="${rowspan}" style="padding:4px; border: 1px solid #ccc;">
                    <strong style="font-size: 12px;">${ka.msg.successfulBid.premium}<br/>(B)</strong>
                </th>
                <th rowspan="${rowspan}" style="padding:4px; border: 1px solid #ccc;">
                    <strong style="font-size: 12px;">${ka.msg.successfulBid.purchasePrice}<br/>(A+B)</strong>
                </th>
                ${
                    this.isKor
                        ? `
                        <th colspan="${colspan}" style="padding:4px; border: 1px solid #ccc; font-size: 12px; font-weight: 700;">
                            <strong style="font-size: 12px;">배송비(C)</strong>
                        </th>
                    `
                        : ''
                }
            </tr>
            ${
                this.isKor
                    ? `
                    <tr style="background-color:#F5F5F5; text-align: center;">
                        <th style="padding:4px; border: 1px solid #ccc">
                            <span style="font-size: 12px; font-weight:400">1.직접 방문 시</span>
                        </th>
                        <th style="padding:4px; border: 1px solid #ccc; font-weight:400">
                            <span style="font-size: 12px;">2.배송 요청 시</span>
                        </th>
                    </tr>
                `
                    : ''
            }
        `;

        let $div = document.createElement('div');
        $div.classList.add('topbox1');
        $div.append($headerTable);
        this.$tableDiv.append($div);

        const $contentDiv = document.createElement('div');
        $contentDiv.classList.add('topbox2');

        const $contentTable = document.createElement('table');
        $contentTable.className = 'bidnoti-table';
        $contentTable.style.width = '100%';
        $contentTable.style.borderCollapse = 'collapse';
        $contentTable.style.fontFamily = '"Noto Sans", sans-serif';
        $contentTable.style.fontSize = '15px';
        $contentTable.style.textAlign = 'right';
        $contentTable.style.borderBottom = '1px solid #ccc';
        $contentTable.cellspacing = '0';
        $contentTable.cellpadding = '4';

        data.AuctionNak.forEach((item, index) => {
            const $container = document.createElement('tr');
            $container.style.backgroundColor = index % 2 === 0 ? '#fff' : '#f9f9f9';
            const workName = item.w_name.length > 10 ? `${item.w_name.substring(0, 10)}...` : item.w_name;

            $container.innerHTML = `
                <td style="width: 7%; padding:4px; border-right: 1px solid #ccc; font-size: 12px; color:#666; text-align: center; border-left: 1px solid #ccc;">
                    ${item.lot_num}
                </td>
                <td style="width: 24%; padding:4px; border-right: 1px solid #ccc; font-size: 12px; color:#666; text-align: left;">
                    <div class="tb-item-name_01">
                        ${item.a_name}
                    </div>
                    <div class="tb-item-name_02">
                        ${workName}
                    </div>
                </td>
                <td style="width: 12%; padding:4px; border-right: 1px solid #ccc; font-size: 12px; color:#666; text-align: right;">
                    ${item.price_successful_bid}
                </td>
                <td style="width: 12%; padding:4px; border-right: 1px solid #ccc; font-size: 12px; color:#666; text-align: right;">
                    ${item.buy_comm_sum}
                </td>
                <td style="width: 12%; padding:4px; border-right: 1px solid #ccc; font-size: 12px; color:#666; text-align: right;">
                    ${item.work_sum}
                </td>
                ${
                    this.isKor
                        ? `
                    <td style="width: 16.5%; padding:4px; border-right: 1px solid #ccc; font-size: 12px; color:#666; text-align: right;">
                        0
                    </td>
                `
                        : ''
                }
                ${
                    this.isKor
                        ? `
                    <td style="width: 16.5%; padding:4px; font-size: 12px; color:#666; text-align: right; border-right: 1px solid #ccc;">
                        ${item.delivery_fee}
                    </td>
                `
                        : ''
                }
            `;

            $contentTable.append($container);
            $contentDiv.append($contentTable);
            this.$tableDiv.append($contentDiv);
        });

        let $tr = document.createElement('tr');
        $tr.style.backgroundColor = '#efefef';
        $tr.style.borderTop = '1px solid #ccc';
        $tr.style.borderBottom = '1px solid #ccc';
        $tr.style.fontWeight = '500';
        $tr.style.color = '#000';

        $tr.innerHTML = `
            <td colspan="2" style="padding:4px; border-left: 1px solid #ccc; border-right: 1px solid #ccc; font-size: 12px; text-align: center;">
                ${ka.msg.successfulBid.total}
            </td>
            <td style="padding:4px; border-right: 1px solid #ccc; font-size: 12px; text-align: right;">
                ${data.total_price_successful_bid}
            </td>
            <td style="padding:4px; border-right: 1px solid #ccc; font-size: 12px; text-align: right;">
                ${data.total_buy_comm_sum}
            </td>
            <td style="padding:4px; border-right: 1px solid #ccc; font-size: 12px; text-align: right;">
                ${data.total_price}
            </td>
            ${
                this.isKor
                    ? `
                <td style="padding:4px; border-right: 1px solid #ccc; font-size: 12px; text-align: right;">
                    0
                </td>
            `
                    : ''
            }
            ${
                this.isKor
                    ? `
                <td style="padding:4px; border-right: 1px solid #ccc; font-size: 12px; color:#000; text-align: right;">
                    ${data.total_delivery_fee}
                </td>
            `
                    : ''
            }
        `;

        $contentTable.append($tr);

        $tr = document.createElement('tr');
        $tr.style.backgroundColor = '#efefef';
        $tr.style.borderTop = '1px solid #ccc';
        $tr.style.borderBottom = '1px solid #ccc';
        $tr.style.fontWeight = '500';
        $tr.innerHTML = `
            <td colspan="${colspan}" style="padding:4px; border-left: 1px solid #ccc; border-right: 1px solid #ccc; font-size: 12px; text-align: center;">
                ${ka.msg.successfulBid.totalAmount}
            </td>
            <td style="padding:4px; border-right: 1px solid #ccc; font-size: 12px; color:#d32f2f; text-align: right;">
                ${data.total_price}
            </td>
            ${
                this.isKor
                    ? `
                <td style="padding:4px; border-right: 1px solid #ccc; font-size: 12px; color:#d32f2f; text-align: right;">
                    ${data.total_price_fee}
                </td>
            `
                    : ''
            }
        `;

        $contentTable.append($tr);
    }

    static #getBidNotification(aucKind, aucNum) {
        return $.ajaxUtils.getApiData('/api/MyPage/GetBidNotification', { auc_kind: aucKind, auc_num: aucNum }, null, false);
    }
}

export class GuaranteeModal extends Modal {
    static WORK_UID_ELEMENT_ID = 'certificate-work-uid';

    constructor(modalId, buttonId, checkboxId, loadDataCallback) {
        super(modalId);
        this.buttonId = buttonId;
        this.checkboxId = checkboxId;
        this.loadDataCallback = loadDataCallback;
    }

    reset() {
        super.reset();
        this.$button = document.querySelector(`#${this.buttonId}`);
        this.$checkbox = document.querySelector(`#${this.checkboxId}`);
    }

    show = async (workUid, state) => {
        this.reset();
        
        const result = await GuaranteeModal.#getCheckTestPrint(workUid);
        const resultCode = $.ajaxUtils.getResultCode(result);
        if (resultCode === '00' || resultCode === '01') {
            this.#setCheckbox();
            this.#setButton();
        } else {
            $.commonUtils.alert(ka.msg.common.error);
            return;
        }

        super.show();

        const $workUid = document.querySelector(`#${GuaranteeModal.WORK_UID_ELEMENT_ID}`);
        $workUid.value = workUid;

        if (state === 'Y') {
            const $printGuarantee = document.querySelector('#print-guarantee');
            $printGuarantee.style.display = 'block';
        } else if (state === 'N') {
            const $notPrintGuarantee = document.querySelector('#not-print-guarantee');
            $notPrintGuarantee.style.display = 'block';
        }
    };
    
    close = async () => {
        await this.loadDataCallback();
        super.close();
    }

    static #getCheckTestPrint(workUid) {
        const param = {
            work_uid: workUid,
        };
        return $.ajaxUtils.getApiData('/api/MyPage/CheckTestPrint', param, null, false);
    }

    static #openReport(testMode) {
        const $workUid = document.querySelector(`#${GuaranteeModal.WORK_UID_ELEMENT_ID}`);
        const param = {
            test_mode: testMode,
            uid: $workUid.value,
        };
        const result = $.ajaxUtils.getApiData('/api/MyPage/SuccessfulBidReport', param, null, false);
        const resultCode = $.ajaxUtils.getResultCode(result);
        if (resultCode === '00') {
            const newWindow = window.open(
                result.message,
                'popWinC',
                'height=400,width=500,toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes'
            );
            if (window.focus) {
                newWindow.focus();
            }
        } else if (resultCode === '80') {
            $.commonUtils.alert(ka.msg.successfulBid.printOut);
        } else {
            $.commonUtils.alert(result.message);
        }
    }

    #setButton() {
        if (DeviceDetector.isMacintosh()) {
            this.$button.addEventListener('click', () => {
                this.#requestDelivery();
            });
            this.$button.textContent = ka.msg.mypage.guaranteeDeliveryRequest;
        } else if (!DeviceDetector.isMacintosh() && !DeviceDetector.isMobile()) {
            this.$button.addEventListener('click', () => {
                GuaranteeModal.#openReport(this.$button.getAttribute('data-print-test'));
            });
            this.$button.textContent = ka.msg.mypage.testPrint;
        }
    }

    #setCheckbox() {
        if (this.$checkbox) {
            // 테스트 출력 이력 여부에 관계 없이 테스트 출력 하도록 기본 설정
            this.$checkbox.checked = false;

            this.$checkbox.addEventListener('click', () => {
                if (this.$checkbox.checked) {
                    this.$button.textContent = ka.msg.mypage.originPrint;
                    this.$button.setAttribute('data-print-test', 'N');
                } else {
                    this.$button.textContent = ka.msg.mypage.testPrint;
                    this.$button.setAttribute('data-print-test', 'Y');
                }
            });
        }
    }

    #requestDelivery() {
        this.close();
        $.commonUtils.modalOpen(ka.msg.mypage.certificateService, 'certificate');
    }
}
