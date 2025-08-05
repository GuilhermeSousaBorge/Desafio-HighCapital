import React, { useEffect, useState } from 'react';
import Sidebar from './Sidebar';
import api from '../api/axios';
import Message from './Message';

export default function ChatWindow() {
  const [selectedBotId, setSelectedBotId] = useState(null);
  const [bots, setBots] = useState([]);
  const [messages, setMessages] = useState({});
  const [input, setInput] = useState('');

  useEffect(() => {
    const get = async () => {
    await api.get('/Bots')
      .then(res => setBots(res.data))
      .catch(err => console.error('Erro ao buscar bots:', err));
    }
    get();
  }, []);

  useEffect(() => {
    if (!selectedBotId) return;
  
    api.get(`/Bots/${selectedBotId}/messages`)
      .then(res => {
        setMessages(prev => ({
          ...prev,
          [selectedBotId]: res.data
        }));
      })
      .catch(err => console.error('Erro ao buscar mensagens:', err));
  }, [selectedBotId]);

  const handleSend = async () => {
    if (!input.trim() || !selectedBotId) return;
  
    const newMessage = {
        content: input.trim()
    };
  
    try {
      const res = await api.post(
        `Messages/Bot/${selectedBotId}/chat`,
        newMessage
      );
      console.log(res);
      // Supondo que o backend responda com TODAS as mensagens atualizadas ou só a nova mensagem + resposta
      const botReply = res.data; // ajusta conforme resposta da API
  
      setMessages(prev => ({
        ...prev,
        [selectedBotId]: [...(prev[selectedBotId] || []), 
        botReply.userMessage, 
        botReply.botMessage]
      }));
  
      setInput('');
    } catch (err) {
      console.error('Erro ao enviar mensagem:', err);
    }
  };
  console.log(messages)
  // const currentMessages = messages[selectedBotId] || [];

  return (
    <div className="flex h-screen">
      <Sidebar bots={bots} onCreateBot={setBots} selectedBotId={selectedBotId} onSelectBot={(bot) => setSelectedBotId(bot.id)} />

      <div className="flex-1 bg-white dark:bg-zinc-950 p-6 flex flex-col">
        {selectedBotId ? (
          <>
            <Message currentMessages={messages[selectedBotId]} />

            <div className="mt-4 flex gap-2">
              <input
                className="flex-1 p-2 rounded bg-zinc-800 text-white"
                placeholder="Digite sua mensagem..."
                value={input}
                onChange={(e) => setInput(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && handleSend()}
              />
              <button
                onClick={handleSend}
                className="bg-green-600 px-4 py-2 rounded hover:bg-green-700"
              >
                Enviar
              </button>
            </div>
          </>
        ) : (
          <div className="text-zinc-500 dark:text-zinc-400 text-xl flex items-center justify-center h-full">
            Selecione um bot para começar a conversa 🧠💬
          </div>
        )}
      </div>
    </div>
  );
}
