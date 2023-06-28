class Template extends HTMLElement {
    shadowRoot;

    constructor() {
        super();
        this.shadowRoot = this.attachShadow({ mode: 'open' });
    }

    applyTemplate(id) {
        const template = document.getElementById(id);
        let clone = template.content.cloneNode(true);
        this.shadowRoot.appendChild(clone);
    }
}

class ChatMessage extends Template {
    constructor() {
        super();
        this.applyTemplate('chat-msg');
    }
}

class ChatChannel extends Template {
    constructor() {
        super();
        this.applyTemplate('chat-channel');
    }
}

const customElementsMap = {
    'chat-msg': ChatMessage,
    'chat-channel': ChatChannel,
};

for (const [selector, elementClass] of Object.entries(customElementsMap)) {
    window.customElements.define(selector, elementClass);
}
