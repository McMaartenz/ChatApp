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

window.customElements.define('chat-msg', ChatMessage);
