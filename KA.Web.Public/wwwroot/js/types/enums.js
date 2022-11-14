export const EAuctionKind = Object.freeze({
    majorAbsentee: '1',
    majorLive: '5',
    premium: '2',
    weekly: '4',
});

export const EWorkInquiryState = Object.freeze({
    enrollInquiry: 'D',
    answerComplete: 'C',
});

export const EConsignmentState = Object.freeze({
    unidentified: '001',
    check: '002',
    review: '006',
    resultSent: '003',
    accept: '004',
    reject: '005'
});

export const EConsignmentViewState = Object.freeze({
   waiting: ['001', '002', '006', '003'],
   possible: ['004'],
   impossible: ['005'], 
});