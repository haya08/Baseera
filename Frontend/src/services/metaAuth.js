export async function startMetaAuth() {
  const response = await fetch("/api/auth/meta/start");

  if (!response.ok) {
    throw new Error("Failed to start Meta authentication");
  }

  return await response.json();
}