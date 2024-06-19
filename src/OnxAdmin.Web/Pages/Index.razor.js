const PROMPT_ID = "prompt";

export function initialize() {
  setupPromptObserver();
}

function getPrompt() {
  return document.getElementById(PROMPT_ID);
}

function setupPromptObserver() {
  const resizeObserver = new ResizeObserver(entries => {
    for (const entry of entries) {
      if (entry.target.id === PROMPT_ID) {
        resizePrompt();
      }
    }
  });
  
  const prompt = getPrompt();
  resizeObserver.observe(prompt);
}

export function resizePrompt() {
  const prompt = getPrompt();
  
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
  const prompt = getPrompt();

  if (prompt === null) {
    return;
  }

  prompt.closest(".prompt-wrapper").style.outline = "2px solid #ffffff";
}

export function blurPrompt() {
  const prompt = getPrompt();

  if (prompt === null) {
    return;
  }

  prompt.closest(".prompt-wrapper").style.outline = "none";
}

export function scrollToBottom() {
  const msgContainer = document.querySelector(".messages-container");

  if (msgContainer === null) {
    return;
  }

  msgContainer.scrollTop = msgContainer.scrollHeight;
}

// TODO: I don't think I need this anymore.
export function resetPrompt() {
  const prompt = getPrompt();

  if (prompt === null) {
    return;
  }

  prompt.style.height = "1.5rem";
  prompt.style.overflow = "hidden";
}

export function openFileInput() {
  const fileInput = document.getElementById("fileInput");
  
  if (fileInput === null) {
    return;
  }

  fileInput.click();
}