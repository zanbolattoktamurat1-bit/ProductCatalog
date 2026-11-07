document.querySelectorAll("form[data-submit-feedback]").forEach((form) => {
  form.addEventListener("submit", () => {
    if (!form.checkValidity()) {
      return;
    }

    const button = form.querySelector(".submit-button");
    if (button) {
      button.disabled = true;
      button.textContent = "Сақталуда...";
      form.setAttribute("aria-busy", "true");
    }
  });
});
