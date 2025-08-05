import React, { useEffect, useState } from "react";
import api from "../api/axios";

export default function Sidebar({ bots, onCreateBot, onSelectBot, selectedBotId }) {
  const [name, setName] = useState("");
  const [context, setContext] = useState("");
  const [loading, setLoading] = useState(false);

  const fetchBots = async () => {
    try {
      setLoading(true);
      const res = await api.get("/Bots");
      console.log("Bots recebidos:", res.data);
      // Se for usar estado local depois:
      // setBots(res.data);
      onCreateBot(res.data);
    } catch (err) {
      console.error("Erro ao buscar bots:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchBots();
  }, []);

  const createBot = async () => {
    if (!name.trim() || !context.trim()) return;
    try {
      await api.post("/Bots", { name, context });
      setName("");
      setContext("");
      fetchBots(); // atualiza a lista após criar
    } catch (err) {
      console.error("Erro ao criar bot:", err);
    }
  };

  if (loading) return <div className="w-64 p-4 bg-zinc-900 text-white">Carregando bots...</div>;

  return (
    <aside className="w-64 bg-zinc-900 text-white p-4 flex flex-col">
      <h2 className="text-lg font-bold mb-4">Meus Chats</h2>

      <div className="flex flex-col gap-2 flex-grow overflow-y-auto">
        {bots?.map((bot) => (
          <button
            key={bot.id}
            onClick={() => onSelectBot(bot)}
            className={`text-left p-2 rounded ${
              selectedBotId === bot.id ? "bg-zinc-700" : "hover:bg-zinc-800"
            }`}
          >
            🤖 {bot.name}
          </button>
        ))}
      </div>

      <div className="mt-4">
        <input
          className="w-full p-1 mb-1 rounded bg-zinc-800 text-sm"
          placeholder="Nome do bot"
          value={name}
          onChange={(e) => setName(e.target.value)}
        />
        <textarea
          className="w-full p-1 mb-1 rounded bg-zinc-800 text-sm"
          placeholder="Contexto"
          value={context}
          onChange={(e) => setContext(e.target.value)}
        />
        <button
          onClick={createBot}
          className="bg-green-600 w-full py-1 rounded text-sm hover:bg-green-700"
        >
          Criar Chat
        </button>
      </div>
    </aside>
  );
}
