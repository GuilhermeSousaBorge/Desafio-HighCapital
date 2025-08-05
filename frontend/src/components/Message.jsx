import React from 'react'

export default function Message({ currentMessages }) {
    return (
        <div className="flex-1 overflow-y-auto space-y-2">
            {currentMessages?.map((msg) => (
                <div
                    key={msg.id}
                    className={`flex px-4 py-2 rounded-lg ${msg.sender === 'User'
                            ? 'bg-blue-600 text-white self-end justify-end'
                            : 'bg-zinc-800 text-white self-start'
                        }`}
                >
                    {msg.content}
                </div>
            ))}
        </div>
    )
}