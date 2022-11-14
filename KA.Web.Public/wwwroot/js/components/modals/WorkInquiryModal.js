import { Modal } from './Modal.js';
import { EWorkInquiryState } from '../../types/enums.js';
import { ImagePathProcessor } from "../../utils/ImagePathProcessor.js";
import { LoadingContent } from "../forms/LoadingContent.js";

export class WorkInquiryModal extends Modal {
    constructor(modalId, getInquiriesCallback) {
        super(modalId);
        this.getInquiriesCallback = getInquiriesCallback;
        this.imagePathProcessor = new ImagePathProcessor();
        this.item = undefined;
        this.$reInquiryBtn = undefined;
    }

    reset() {
        super.reset();
        this.$form = this.$modal.querySelector('form');
        this.$message = this.$modal.querySelector('#inquiry-message');
        this.$categorySelect = this.$modal.querySelector('#modal__inquiry-category');
        this.$submitBtn = this.$modal.querySelector('#inquiry-submit-btn');
        this.$reInquiryBtn = this.$modal.querySelector('#re-inquiry-btn');
        this.loadingBtn = new LoadingContent(this.$submitBtn);
    }

    init(item) {
        this.item = item;
        this.$form.reset();

        // 모달 Display 데이터 초기화
        this.$modal.querySelector('#modal__inquiry-auction-title').textContent = `${item.auc_title}-LOT${item.lot_num}`;
        this.$modal.querySelector('#modal__inquiry-work-title').textContent = `${item.artist_name} / ${item.title}`;
        this.$modal.querySelector('#modal__inquiry-work-image').src = this.imagePathProcessor.processAuctionImage(item.img_file_name, item.auc_kind, item.auc_num);
        
        // 카테고리 설정
        const $options = this.$categorySelect.querySelectorAll('option');
        $options[0].selected = true;

        $options.forEach((option) => {
            if (option.value === item.category) {
                option.selected = true;
            }
        });

        // 메세지 설정
        this.$message.textContent = item.contents ?? '';

        this.#setAction(item);
    }

    show = (item) => {
        this.reset();
        this.init(item);
        super.show();
    };
    
    close = () => {
        super.close();
    }

    #setAction(item) {
        const $lengthSpan = this.$modal.querySelector('#inquiry-length');
        $lengthSpan.textContent = `(${item?.contents?.length?.toString() ?? 0}/1,000)`;

        this.$message.addEventListener('keyup', function (e) {
            e.preventDefault();
            e.stopPropagation();
            let message = this.value;
            if (message.length > 1000) {
                $.commonUtils.alert(ka.msg.inquiry.lengthCheck);
                message = message.substring(0, 1000);
                this.value = message;
            }
            $lengthSpan.textContent = `(${message.length.toString() ?? 0}/1,000)`;
        });

        const $state = this.$modal.querySelector('.state');
        const $inquiryHintMessage = this.$modal.querySelector('#inquiry-hint-message');
        
        switch (item.state) {
            case undefined: 
            case null:
                $state.style.visibility = 'hidden';
                $inquiryHintMessage.style.visibility = 'visible';
                $lengthSpan.style.visibility = 'visible';
                this.$message.removeAttribute('disabled');
                this.$categorySelect.removeAttribute('disabled');
                this.$submitBtn.textContent = ka.msg.common.submit;
                this.$submitBtn.addEventListener('click', this.submitEvent);
                if (this.$reInquiryBtn) {
                    this.$reInquiryBtn.style.display = 'none';
                }
                break;
            case EWorkInquiryState.enrollInquiry:
                $state.style.visibility = 'visible';
                $state.textContent = ka.msg.mypage.waitingAnswer;
                $inquiryHintMessage.style.visibility = 'hidden';
                $lengthSpan.style.visibility = 'visible';
                this.$message.removeAttribute('disabled');
                this.$categorySelect.setAttribute('disabled', 'disabled');
                this.$submitBtn.textContent = ka.msg.common.save;
                this.$submitBtn.addEventListener('click', this.submitEvent);
                if (this.$reInquiryBtn) {
                    this.$reInquiryBtn.style.display = 'block';
                    this.$reInquiryBtn.addEventListener('click', this.reInquiryEvent);
                }
                break;
            case EWorkInquiryState.answerComplete:
                $state.style.visibility = 'visible';
                $state.textContent = ka.msg.mypage.answerComplete;
                $inquiryHintMessage.style.visibility = 'hidden';
                $lengthSpan.style.visibility = 'hidden';
                this.$message.setAttribute('disabled', 'disabled');
                this.$categorySelect.setAttribute('disabled', 'disabled');
                this.$submitBtn.textContent = ka.msg.common.confirm;
                this.$submitBtn.addEventListener('click', this.close);
                if (this.$reInquiryBtn) {
                    this.$reInquiryBtn.style.display = 'block';
                    this.$reInquiryBtn.addEventListener('click', this.reInquiryEvent);
                }
                break;
        }
    }

    submitEvent = async () => {
        await this.#createOrUpdate(this.item);
    };

    reInquiryEvent = () => {
        this.reset();
        this.init({
            work_uid: this.item.work_uid,
            title: this.item.title,
            auc_title: this.item.auc_title,
            lot_num: this.item.lot_num,
            artist_name: this.item.artist_name,
            auc_kind: this.item.auc_kind,
            auc_num: this.item.auc_num,
            img_file_name: this.item.img_file_name,
        });
    };

    async #createOrUpdate(item) {
        const contents = this.$message?.value;
        if (item.uid) {
            if (this.$message.value === item.contents) {
                $.commonUtils.alert(ka.msg.common.invalidInput);
                return;
            }

            this.loadingBtn.start();
            await this.#postInquiry({
                uid: item.uid,
                work_uid: item.work_uid,
                category: item.category,
                contents: contents,
                type: 'W',
            });
            this.loadingBtn.end();
        } else {
            const category = this.$categorySelect.value;

            if (!this.$message.value || !category) {
                $.commonUtils.alert(ka.msg.common.invalidInput);
                return;
            }
            this.loadingBtn.start();
            await this.#postInquiry({
                work_uid: item.work_uid,
                contents: contents,
                category: category,
                type: 'W',
            });
            this.loadingBtn.end();
        }

        if (this.getInquiriesCallback) {
            await this.getInquiriesCallback();
        }
    }

    async #postInquiry(parameters) {
        try {
            const response = await fetch('/api/Member/Inquiry', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(parameters),
            });
            const result = await response.json();
            if (result.code === '00') {
                $.commonUtils.alert(ka.msg.inquiry.complete, 'success');
            } else {
                $.commonUtils.alert(ka.msg.common.error);
            }
        } catch (e) {
            console.log(e.description);
        } finally {
            this.close();
        }
    }
}
