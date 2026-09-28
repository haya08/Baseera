const API_BASE_URL = "";

export async function startMetaAuth() {
  const response = await fetch("/api/auth/meta/start");

  if (!response.ok) {
    throw new Error("Failed to start Meta authentication");
  }

  return response.json();
}

export async function checkMetaConnection(requestId) {
  const response = await fetch(
    `/api/auth/meta/connection?requestId=${encodeURIComponent(requestId)}`
  );

  if (!response.ok) {
    throw new Error("Failed to check Meta connection");
  }

  return response.json();
}