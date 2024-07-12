#!/bin/bash

/bin/ollama serve & pid=$!

sleep 5

echo "🔴 Retrieve  model..."
ollama pull nomic-embed-text
echo "🟢 Done!"

wait $pid