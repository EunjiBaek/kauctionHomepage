//ES6 테스트 구조체
const address = {
    contry: "한국",
    city: "서울",
    street: "압구정로"
};

const { contry, city } = address;

//ES6 샘플코드
document.querySelectorAll("li").forEach((el) => {
    const p = `<p>${contry}, ${city}</p>`;
    el.insertAdjacentHTML("beforeend", p);
});