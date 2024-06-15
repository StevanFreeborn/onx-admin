export function resizePrompt() {
  console.log("resizePrompt");
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

  prompt.closest("form").style.outline = "2px solid #ffffff";
}

export function blurPrompt() {
  const prompt = document.getElementById("prompt");

  if (prompt === null) {
    return;
  }

  prompt.closest("form").style.outline = "none";
}
