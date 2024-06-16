export function resizePrompt() {
  const prompt = document.getElementById("prompt");

  if (prompt === null) {
    return;
  }

  prompt.style.height = "auto";

  const scrollHeight = prompt.scrollHeight;

  if (scrollHeight > 300) {
    prompt.style.height = "300px";
    prompt.style.overflow = "auto";
    return;
  }

  prompt.style.height = `${scrollHeight}px`;
  prompt.style.overflow = "hidden";
}

export function focusPrompt() {
  const prompt = document.getElementById("prompt");

  if (prompt === null) {
    return;
  }

  prompt.closest(".prompt-wrapper").style.outline = "2px solid #ffffff";
}

export function blurPrompt() {
  const prompt = document.getElementById("prompt");

  if (prompt === null) {
    return;
  }

  prompt.closest(".prompt-wrapper").style.outline = "none";
}

export function scrollToBottom() {
  console.log("scrollToBottom");
  const msgContainer = document.querySelector(".messages-container");

  if (msgContainer === null) {
    return;
  }

  msgContainer.scrollTop = msgContainer.scrollHeight;
}

export function resetPrompt() {
  const prompt = document.getElementById("prompt");

  if (prompt === null) {
    return;
  }

  prompt.style.height = "1.5rem";
  prompt.style.overflow = "hidden";
}
