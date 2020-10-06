import Vue from "vue";

class MessageService extends Vue {

  constructor() {
    super();
  }
}

export const messageService = new MessageService();

messageService.$emit('miomessaggio', {})
messageService.$on('miomessaggio', (arg:any) => { console.log(arg) });