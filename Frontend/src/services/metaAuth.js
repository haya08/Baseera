const API_BASE_URL = "";

export async function startMetaAuth() {
  const response = await fetch(
    `${API_BASE_URL}/api/auth/meta/start`
  );

  if (!response.ok) {
    throw new Error(
      "Failed to start Meta authentication"
    );
  }

  return response.json();
}

export async function checkMetaConnection(requestId) {
  const response = await fetch(
    `${API_BASE_URL}/api/auth/meta/connection?requestId=${encodeURIComponent(
      requestId
    )}`
  );

  if (!response.ok) {
    throw new Error(
      "Failed to check Meta connection"
    );
  }

  return response.json();
}

export async function collectFacebookData(url) {
  const response = await fetch(
    `${API_BASE_URL}/api/connectors/facebook`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        url,
      }),
    }
  );

  if (!response.ok) {
    const message = await response.text();

    throw new Error(
      message || "Failed to collect Facebook data"
    );
  }

  return response.json();
}

export async function collectInstagramData(url) {
  const response = await fetch(
    `${API_BASE_URL}/api/connectors/instagram`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        url,
      }),
    }
  );

  if (!response.ok) {
    const message = await response.text();

    throw new Error(
      message || "Failed to collect Instagram data"
    );
  }

  return response.json();
}