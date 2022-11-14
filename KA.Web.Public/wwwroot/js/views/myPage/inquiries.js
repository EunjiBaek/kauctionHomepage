import { DatePicker } from '../../components/forms/DatePicker.js';
import { ContentTab } from '../../components/buttons/ContentTab.js';
import { MultipleCheckboxGroup } from '../../components/forms/MultipleCheckboxGroup.js';
import { SearchInput } from '../../components/forms/SearchInput.js';
import { ImagePathProcessor } from '../../utils/ImagePathProcessor.js';
import { RouteValidator } from '../../utils/RouteValidator.js';
import { WorkInquiryModal } from '../../components/modals/WorkInquiryModal.js';
import { EConsignmentState, EConsignmentViewState, EWorkInquiryState } from '../../types/enums.js';
import { ResetButton } from '../../components/buttons/ResetButton.js';

$(async function () {
    'use strict';
    const consignmentDatePicker = new DatePicker('consignment-date-picker', 1);
    const workDatePicker = new DatePicker('work-date-picker', 1);

    const consignmentTab = new ContentTab('consignment-li');
    const workTab = new ContentTab('work-li');

    consignmentTab.setEvent('/myPage/inquiries/consignments');
    workTab.setEvent('/myPage/inquiries/works');

    const consignmentStateCheckboxes = new ConsignmentCheckboxGroup('consignment_state_check', getMyConsignmentInquiries);
    const workStateCheckboxes = new MultipleCheckboxGroup('work_state_check', getMyWorkInquiries);

    consignmentStateCheckboxes.setEvent();
    workStateCheckboxes.setEvent();

    const consignmentSearchInput = new SearchInput('search-consignment-input', 'search-consignment-btn', getMyConsignmentInquiries);
    const workSearchInput = new SearchInput('search-work-input', 'search-work-btn', getMyWorkInquiries);
    consignmentSearchInput.setEvent();
    workSearchInput.setEvent();

    const consignmentResetBtn = new ResetButton('consignment-reset-btn');
    consignmentResetBtn.setEvent(getMyConsignmentInquiries, null, consignmentDatePicker.init, consignmentStateCheckboxes.init, consignmentSearchInput.init);

    const workResetBtn = new ResetButton('work-reset-btn');
    workResetBtn.setEvent(getMyWorkInquiries, null, workDatePicker.init, workStateCheckboxes.init, workSearchInput.init);

    if (RouteValidator.lastSegmentStartsWith('consignments')) {
        consignmentTab.selectTab();
        await getMyConsignmentInquiries();
    } else if (RouteValidator.lastSegmentStartsWith('works')) {
        workTab.selectTab();
        await getMyWorkInquiries();
    } else {
        console.error('잘못된 경로 입니다.');
        consignmentTab.selectTab();
        await getMyConsignmentInquiries();
    }

    const consignmentItemsDiv = new ConsignmentInquiryItemsDiv('consignment-items-div');
    const workInquiryModal = new WorkInquiryModal('inquiry_popup', getMyWorkInquiries);
    const workItemsDiv = new WorkInquiryItemsDiv('work-items-div', workInquiryModal.show);

    async function getMyConsignmentInquiries(parameters) {
        const [startDate, endDate] = consignmentDatePicker.getSearchDateRange();
        const apiParameters = {
            start_date: startDate,
            end_date: endDate,
            states: consignmentStateCheckboxes.getValues(),
            search: consignmentSearchInput.getSearchText(),
            ...parameters,
        };

        await $.ajaxUtils.getApiData('/api/MyPage/ConsignList', apiParameters, getMyConsignmentInquiriesComplete);
    }

    function getMyConsignmentInquiriesComplete(result) {
        if ($.ajaxUtils.getResultCode(result) !== '00') {
            $.commonUtils.alert(ka.msg.common.error);
        }

        consignmentItemsDiv.clear();
        result?.data?.data?.forEach((item) => {
            consignmentItemsDiv.renderDiv(item);
        });

        document.querySelector('#consignment-item-count').textContent = `(${result?.data?.recordsTotal})`;

        if (result.data.recordsTotal === 0) {
            consignmentItemsDiv.showEmpty('조회된 정보가 없습니다.');
        }
    }

    async function getMyWorkInquiries(parameters) {
        const [startDate, endDate] = workDatePicker.getSearchDateRange();
        const apiParameters = {
            start_date: startDate,
            end_date: endDate,
            states: workStateCheckboxes.getCheckedValues(),
            search: workSearchInput.getSearchText(),
            ...parameters,
        };

        await $.ajaxUtils.getApiData('/api/MyPage/GetInquiry', apiParameters, getMyWorkInquiriesComplete);
    }

    function getMyWorkInquiriesComplete(result) {
        if ($.ajaxUtils.getResultCode(result) !== '00') {
            $.commonUtils.alert(ka.msg.common.error);
        }

        workItemsDiv.clear();
        result?.data?.data?.forEach((item) => {
            workItemsDiv.renderDiv(item);
        });

        document.querySelector('#inquiry-item-count').textContent = `(${result?.data?.recordsTotal})`;

        if (result?.data?.recordsTotal === 0) {
            workItemsDiv.showEmpty('조회된 정보가 없습니다.');
        }
    }
});

export class ConsignmentCheckboxGroup extends MultipleCheckboxGroup {
    constructor(checkboxName, getDataCallback) {
        super(checkboxName, getDataCallback);
    }

    getValues() {
        const viewValues = this.getCheckedValues();
        let values = [];
        viewValues.forEach((value) => {
            values = [...values, ...EConsignmentViewState[value]];
        });
        return values;
    }
}

export class InquiryItemsDiv {
    constructor(divId) {
        this.$div = document.querySelector(`#${divId}`);
        this.$emptyDiv = document.querySelector(`#${divId}-empty`);
        this.imagePathProcessor = new ImagePathProcessor();
    }

    clear() {
        this.$div.innerHTML = '';
        this.$emptyDiv.style.display = 'none';
    }

    showEmpty() {
        this.$emptyDiv.style.display = 'block';
    }
}

export class ConsignmentInquiryItemsDiv extends InquiryItemsDiv {
    constructor(divId) {
        super(divId);
    }

    renderDiv = (item) => {
        const $div = document.createElement('div');
        if (item.state === EConsignmentState.accept || item.state === EConsignmentState.reject) {
            $div.classList.add('complete');
        }
        const fileName = item.images.split('^')[0];
        const regDateSplit = item.reg_date_full?.split(' ') ?? ['', ''];
        const image = this.imagePathProcessor.processConsignmentImage(fileName);

        let stateName = '';
        let stateClass = '';

        switch (item.state) {
            case EConsignmentState.unidentified:
            case EConsignmentState.check:
            case EConsignmentState.review:
            case EConsignmentState.resultSent:
                stateName = ka.msg.mypage.waiting;
                stateClass = "waiting";
                break;
            case EConsignmentState.accept:
                stateName = ka.msg.mypage.consignmentPossible;
                stateClass = "consignmentPossible";
                break;
            case EConsignmentState.reject:
                stateName = ka.msg.mypage.consignmentImpossible;
                stateClass = "consignmentImpossible";
                break;
            default:
                stateName = ka.msg.mypage.waiting;
                stateClass = "waiting";
                console.error('unknown state type');
                break;
        }

        $div.innerHTML = `
            <figure>
                <div>
                    <div class="img" style="background-image: url(${image});"></div>
                    <figcaption class="${stateClass}">
                        ${stateName}
                        <span>${item.review_date ?? ''}</span>
                    </figcaption>
                </div>
            </figure>
            <div class="info">
                <div class="w_info">
                    <span class="w_name">${item.artist}</span>
                    <span class="w_tit">${item.title}</span>
                </div>
    
                <p>등록일 : ${regDateSplit[0]}</p>
            </div>
        `;

        $div.querySelector('figure').addEventListener('click', () => {
            window.location.href = `/MyPage/Consign/${item.uid}`;
        });

        this.$div.append($div);
    };
}

export class WorkInquiryItemsDiv extends InquiryItemsDiv {
    constructor(divId, showDetailCallback) {
        super(divId);
        this.showDetailCallback = showDetailCallback;
    }

    renderDiv = (item) => {
        const $div = document.createElement('div');
        $div.classList.add('box');
        
        let stateName = '';
        let stateClass = '';

        switch (item.state) {
            case EWorkInquiryState.enrollInquiry:
                stateName = ka.msg.mypage.waitingAnswer;
                stateClass = "waitingAnswer";
                break;
            case EWorkInquiryState.answerComplete:
                stateName = ka.msg.mypage.answerComplete;
                stateClass = "answerComplete";
                break;
            default:
                console.error('Unknown state type.');
                stateName = ka.msg.mypage.waitingAnswer;
                stateClass = "waitingAnswer";
                break;
        }
        
        $div.innerHTML = `
            <span class="state">
                ${stateName}
            </span>
            <div>
                <div class="img" style="background-image: url(${this.imagePathProcessor.processAuctionImage(item.img_file_name, item.auc_kind, item.auc_num)});">
                </div>
                <div class="content">
                    <p class="auc_detail">${item.auc_title} - LOT${item.lot_num}</p>
                    <div class="w_detail">
                        <strong>${item.artist_name ?? ka.msg.mypage.unidentified}</strong>
                        <span>${item.title ?? ka.msg.mypage.unidentified}</span>
                    </div>
                    <div class="detail_content">
                        ${item.contents}
                    </div>
                    <p class="date">${item.reg_date} ${ka.msg.mypage.registration}</p>
                </div>
            </div>
        `;

        $div.addEventListener('click', () => {
            this.showDetailCallback(item);
        });

        this.$div.append($div);
    };
}
