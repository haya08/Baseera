import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  CircularProgress,
  Grid,
  Stack,
  TextField,
  Typography,
} from "@mui/material";

import HubIcon from "@mui/icons-material/Hub";
import FacebookIcon from "@mui/icons-material/Facebook";
import InstagramIcon from "@mui/icons-material/Instagram";
import RedditIcon from "@mui/icons-material/Language";
import StorageIcon from "@mui/icons-material/Storage";
import LinkIcon from "@mui/icons-material/Link";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import RefreshIcon from "@mui/icons-material/Refresh";

import { useState } from "react";

import {
  startMetaAuth,
  checkMetaConnection,
  collectFacebookData,
  collectInstagramData,
} from "../services/metaAuth";

// =========================
// Social Sources
// =========================

const socialSources = [
  {
    name: "Facebook",
    description:
      "Collect posts and content from your managed Facebook Pages.",
    type: "Facebook Pages",
    icon: <FacebookIcon sx={{ fontSize: 32 }} />,
  },
  {
    name: "Instagram",
    description:
      "Collect media and comments from your connected Instagram Business account.",
    type: "Instagram Business",
    icon: <InstagramIcon sx={{ fontSize: 32 }} />,
  },
];

// =========================
// Other Sources
// =========================

const otherSources = [
  {
    name: "Reddit",
    description:
      "Collect public discussions and community content.",
    type: "Reddit",
    icon: <RedditIcon sx={{ fontSize: 30 }} />,
  },
  {
    name: "Bluesky",
    description:
      "Collect social content and conversations from Bluesky.",
    type: "Bluesky",
    icon: <LinkIcon sx={{ fontSize: 30 }} />,
  },
  {
    name: "Mastodon",
    description:
      "Collect social content from connected Mastodon sources.",
    type: "Mastodon",
    icon: <StorageIcon sx={{ fontSize: 30 }} />,
  },
];

// =========================
// Source Card
// =========================

function SourceCard({
  source,
  waitingForMeta = false,
  connected = false,
  onCollect,
  collectingSource,
}) {
  const isFacebook = source.name === "Facebook";

  const defaultUrl = isFacebook
    ? "https://www.facebook.com/Baseera"
    : "https://www.instagram.com/";

  const [url, setUrl] = useState(defaultUrl);

  const isCollecting =
    collectingSource === source.name.toLowerCase();

  return (
    <Card
      sx={{
        height: "100%",
        borderRadius: 3,
        border: "1px solid",
        borderColor: connected
          ? "success.main"
          : "divider",
        boxShadow: "none",
        transition: "all 0.2s ease",

        "&:hover": {
          transform: "translateY(-3px)",
          boxShadow:
            "0 8px 24px rgba(0,0,0,0.08)",
          borderColor: connected
            ? "success.main"
            : "primary.light",
        },
      }}
    >
      <CardContent sx={{ p: 3 }}>
        {/* Header */}

        <Stack
          direction="row"
          justifyContent="space-between"
          alignItems="flex-start"
        >
          <Stack
            direction="row"
            spacing={2}
            alignItems="center"
          >
            <Box
              sx={{
                width: 56,
                height: 56,
                borderRadius: 2,
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                bgcolor: connected
                  ? "success.light"
                  : "action.hover",
                color: connected
                  ? "success.dark"
                  : "text.primary",
              }}
            >
              {source.icon}
            </Box>

            <Box>
              <Typography
                variant="h6"
                fontWeight={700}
              >
                {source.name}
              </Typography>

              <Typography
                variant="body2"
                color="text.secondary"
                sx={{ mt: 0.3 }}
              >
                {source.type}
              </Typography>
            </Box>
          </Stack>

          <Chip
            label={
              connected
                ? "Connected"
                : waitingForMeta
                ? "Waiting for Meta"
                : "Not Connected"
            }
            size="small"
            color={
              connected ? "success" : "default"
            }
            variant="outlined"
          />
        </Stack>

        {/* Description */}

        <Typography
          color="text.secondary"
          sx={{
            mt: 3,
            minHeight: 52,
            lineHeight: 1.6,
          }}
        >
          {source.description}
        </Typography>

        {/* Waiting For Meta */}

        {waitingForMeta && !connected ? (
          <Box
            sx={{
              mt: 3,
              py: 1.15,
              px: 2,
              borderRadius: 2,
              bgcolor: "action.hover",
              textAlign: "center",
            }}
          >
            <Typography
              variant="body2"
              color="text.secondary"
              fontWeight={500}
            >
              Connect your Meta account first
            </Typography>
          </Box>
        ) : connected ? (
          /* Connected Source */

          <Box sx={{ mt: 3 }}>
            <TextField
              fullWidth
              size="small"
              label={`${source.name} URL`}
              value={url}
              onChange={(e) =>
                setUrl(e.target.value)
              }
            />

            <Button
              variant="contained"
              fullWidth
              startIcon={
                isCollecting ? (
                  <CircularProgress
                    size={18}
                    color="inherit"
                  />
                ) : (
                  <LinkIcon />
                )
              }
              disabled={isCollecting}
              onClick={() =>
                onCollect(
                  source.name.toLowerCase(),
                  url
                )
              }
              sx={{
                mt: 2,
                borderRadius: 2,
                textTransform: "none",
                fontWeight: 600,
              }}
            >
              {isCollecting
                ? "Collecting..."
                : "Collect Data"}
            </Button>
          </Box>
        ) : (
          /* Not Connected */

          <Button
            variant="outlined"
            fullWidth
            startIcon={<LinkIcon />}
            sx={{
              mt: 3,
              borderRadius: 2,
              textTransform: "none",
              fontWeight: 600,
            }}
          >
            Connect
          </Button>
        )}
      </CardContent>
    </Card>
  );
}

// =========================
// Data Sources Page
// =========================

function DataSources() {
  const [isConnecting, setIsConnecting] =
    useState(false);

  const [isChecking, setIsChecking] =
    useState(false);

  const [metaConnected, setMetaConnected] =
    useState(false);

  const [metaPages, setMetaPages] =
    useState([]);

  const [error, setError] = useState("");

  const [collectionMessage, setCollectionMessage] =
    useState("");

  const [collectingSource, setCollectingSource] =
    useState("");

  // =========================
  // Start Meta Authentication
  // =========================

  const handleConnectMeta = async () => {
    // Open window immediately so browser
    // does not block the popup.
    const metaWindow = window.open(
      "about:blank",
      "_blank"
    );

    try {
      setIsConnecting(true);
      setError("");
      setCollectionMessage("");

      const data = await startMetaAuth();

      if (!data.authorizationUrl) {
        throw new Error(
          "Meta authorization URL was not returned"
        );
      }

      if (data.requestId) {
        sessionStorage.setItem(
          "baseera_meta_request_id",
          data.requestId
        );
      }

      if (metaWindow) {
        metaWindow.location.href =
          data.authorizationUrl;
      } else {
        // Fallback if popup was blocked.
        window.location.href =
          data.authorizationUrl;
      }
    } catch (err) {
      console.error(
        "Meta authentication error:",
        err
      );

      if (metaWindow) {
        metaWindow.close();
      }

      setError(
        err?.message ||
          "Unable to start Meta authentication."
      );
    } finally {
      setIsConnecting(false);
    }
  };

  // =========================
  // Check Meta Connection
  // =========================

  const handleCheckConnection = async () => {
    try {
      setIsChecking(true);
      setError("");
      setCollectionMessage("");

      const requestId =
        sessionStorage.getItem(
          "baseera_meta_request_id"
        );

      if (!requestId) {
        throw new Error(
          "No Meta request ID found."
        );
      }

      const data =
        await checkMetaConnection(requestId);

      if (!data.connected) {
        setMetaConnected(false);
        setMetaPages([]);

        setError(
          "Meta is not connected yet. Complete the Facebook authorization first."
        );

        return;
      }

      setMetaConnected(true);
      setMetaPages(data.pages || []);

      setCollectionMessage(
        "Meta connected successfully. Your available sources are ready."
      );
    } catch (err) {
      console.error(
        "Meta connection error:",
        err
      );

      setError(
        err?.message ||
          "Unable to check Meta connection. Please try again."
      );
    } finally {
      setIsChecking(false);
    }
  };

  // =========================
  // Collect Data
  // =========================

  const handleCollect = async (
    source,
    url
  ) => {
    try {
      setCollectingSource(source);
      setError("");
      setCollectionMessage("");

      if (!url.trim()) {
        throw new Error(
          `Please provide a ${source} URL.`
        );
      }

      if (source === "facebook") {
        await collectFacebookData(url);

        setCollectionMessage(
          "Facebook data collection completed successfully."
        );
      }

      if (source === "instagram") {
        await collectInstagramData(url);

        setCollectionMessage(
          "Instagram data collection completed successfully."
        );
      }
    } catch (err) {
      console.error(
        "Collection error:",
        err
      );

      setError(
        err?.message ||
          `Unable to collect ${source} data.`
      );
    } finally {
      setCollectingSource("");
    }
  };

  // =========================
  // Render
  // =========================

  return (
    <Box>
      {/* =========================
          Page Header
      ========================= */}

      <Box sx={{ mb: 4 }}>
        <Typography
          variant="h4"
          fontWeight={700}
          gutterBottom
        >
          Data Sources
        </Typography>

        <Typography color="text.secondary">
          Connect your social platforms and
          collect data for the Baseera
          knowledge pipeline.
        </Typography>
      </Box>

      {/* =========================
          Error Message
      ========================= */}

      {error && (
        <Alert
          severity="error"
          sx={{
            mb: 3,
            borderRadius: 2,
          }}
        >
          {error}
        </Alert>
      )}

      {/* =========================
          Success Message
      ========================= */}

      {collectionMessage && (
        <Alert
          severity="success"
          sx={{
            mb: 3,
            borderRadius: 2,
          }}
        >
          {collectionMessage}
        </Alert>
      )}

      {/* =========================
          Meta Authentication
      ========================= */}

      <Card
        sx={{
          mb: 5,
          borderRadius: 3,
          border: "1px solid",
          borderColor: metaConnected
            ? "success.main"
            : "primary.main",
          boxShadow: "none",

          background: metaConnected
            ? "linear-gradient(135deg, rgba(46,125,50,0.06), rgba(255,255,255,1))"
            : "linear-gradient(135deg, rgba(25,118,210,0.06), rgba(255,255,255,1))",
        }}
      >
        <CardContent
          sx={{
            p: {
              xs: 3,
              md: 4,
            },
          }}
        >
          <Stack
            direction={{
              xs: "column",
              md: "row",
            }}
            justifyContent="space-between"
            alignItems={{
              xs: "flex-start",
              md: "center",
            }}
            spacing={3}
          >
            {/* Meta Info */}

            <Stack
              direction="row"
              spacing={2}
              alignItems="center"
            >
              <Box
                sx={{
                  width: 64,
                  height: 64,
                  borderRadius: 2.5,
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",

                  bgcolor: metaConnected
                    ? "success.main"
                    : "primary.main",

                  color: "white",
                }}
              >
                {metaConnected ? (
                  <CheckCircleIcon
                    sx={{ fontSize: 34 }}
                  />
                ) : (
                  <HubIcon
                    sx={{ fontSize: 34 }}
                  />
                )}
              </Box>

              <Box>
                <Stack
                  direction="row"
                  spacing={1}
                  alignItems="center"
                  flexWrap="wrap"
                >
                  <Typography
                    variant="h6"
                    fontWeight={700}
                  >
                    Meta Account
                  </Typography>

                  <Chip
                    label={
                      metaConnected
                        ? "Connected"
                        : "Authentication"
                    }
                    size="small"
                    color={
                      metaConnected
                        ? "success"
                        : "primary"
                    }
                  />
                </Stack>

                <Typography
                  color="text.secondary"
                  sx={{ mt: 0.5 }}
                >
                  Facebook Pages & Instagram
                  Business
                </Typography>
              </Box>
            </Stack>

            {/* Meta Actions */}

            <Stack
              direction={{
                xs: "column",
                sm: "row",
              }}
              spacing={2}
            >
              {!metaConnected && (
                <Button
                  variant="contained"
                  size="large"
                  startIcon={<LinkIcon />}
                  onClick={handleConnectMeta}
                  disabled={isConnecting}
                  sx={{
                    borderRadius: 2,
                    textTransform: "none",
                    fontWeight: 600,
                    px: 3,
                  }}
                >
                  {isConnecting
                    ? "Connecting..."
                    : "Connect Meta"}
                </Button>
              )}

              <Button
                variant="outlined"
                size="large"
                startIcon={<RefreshIcon />}
                onClick={
                  handleCheckConnection
                }
                disabled={isChecking}
                sx={{
                  borderRadius: 2,
                  textTransform: "none",
                  fontWeight: 600,
                }}
              >
                {isChecking
                  ? "Checking..."
                  : "Check Connection"}
              </Button>
            </Stack>
          </Stack>

          {/* Meta Description */}

          <Box
            sx={{
              mt: 3,
              pt: 3,
              borderTop: "1px solid",
              borderColor: "divider",
            }}
          >
            <Typography
              color="text.secondary"
              lineHeight={1.7}
            >
              {metaConnected
                ? "Your Meta account is connected. Baseera discovered the available social sources."
                : "Connect your Meta business account once. Baseera will then discover your managed Facebook Pages and connected Instagram Business accounts."}
            </Typography>
          </Box>
        </CardContent>
      </Card>

      {/* =========================
          Connected Meta Pages
      ========================= */}

      {metaConnected &&
        metaPages.length > 0 && (
          <Box sx={{ mb: 5 }}>
            <Box sx={{ mb: 2 }}>
              <Typography
                variant="h6"
                fontWeight={700}
              >
                Connected Meta Sources
              </Typography>

              <Typography
                variant="body2"
                color="text.secondary"
                sx={{ mt: 0.5 }}
              >
                Sources discovered from your
                Meta account.
              </Typography>
            </Box>

            <Grid
              container
              spacing={3}
            >
              {metaPages.map(
                (page, index) => (
                  <Grid
                    item
                    xs={12}
                    md={6}
                    key={
                      page.pageId ||
                      page.id ||
                      index
                    }
                  >
                    <Card
                      sx={{
                        borderRadius: 3,
                        border: "1px solid",
                        borderColor:
                          "success.light",
                        boxShadow: "none",
                      }}
                    >
                      <CardContent
                        sx={{ p: 3 }}
                      >
                        <Stack
                          direction="row"
                          spacing={2}
                          alignItems="center"
                        >
                          <Box
                            sx={{
                              width: 50,
                              height: 50,
                              borderRadius: 2,
                              display: "flex",
                              alignItems:
                                "center",
                              justifyContent:
                                "center",
                              bgcolor:
                                "success.light",
                              color:
                                "success.dark",
                            }}
                          >
                            <FacebookIcon />
                          </Box>

                          <Box>
                            <Typography
                              variant="h6"
                              fontWeight={700}
                            >
                              {page.pageName ||
                                page.name ||
                                "Facebook Page"}
                            </Typography>

                            <Typography
                              variant="body2"
                              color="text.secondary"
                            >
                              Connected Meta
                              Page
                            </Typography>

                            {page.instagramBusinessAccountId && (
                              <Typography
                                variant="body2"
                                color="success.main"
                                sx={{ mt: 0.5 }}
                              >
                                Instagram Business
                                Account connected
                              </Typography>
                            )}
                          </Box>
                        </Stack>
                      </CardContent>
                    </Card>
                  </Grid>
                )
              )}
            </Grid>
          </Box>
        )}

      {/* =========================
          Social Sources
      ========================= */}

      <Box sx={{ mb: 2 }}>
        <Typography
          variant="h6"
          fontWeight={700}
        >
          Social Sources
        </Typography>

        <Typography
          variant="body2"
          color="text.secondary"
          sx={{ mt: 0.5 }}
        >
          Each platform is handled by its
          own data connector.
        </Typography>
      </Box>

      <Grid
        container
        spacing={3}
        sx={{ mb: 5 }}
      >
        {socialSources.map((source) => (
          <Grid
            item
            xs={12}
            md={6}
            key={source.name}
          >
            <SourceCard
              source={source}
              waitingForMeta={
                !metaConnected
              }
              connected={metaConnected}
              onCollect={handleCollect}
              collectingSource={
                collectingSource
              }
            />
          </Grid>
        ))}
      </Grid>

      {/* =========================
          Other Sources
      ========================= */}

      <Box sx={{ mb: 2 }}>
        <Typography
          variant="h6"
          fontWeight={700}
        >
          Other Sources
        </Typography>

        <Typography
          variant="body2"
          color="text.secondary"
          sx={{ mt: 0.5 }}
        >
          Additional social platforms
          supported by Baseera.
        </Typography>
      </Box>

      <Grid container spacing={3}>
        {otherSources.map((source) => (
          <Grid
            item
            xs={12}
            md={6}
            lg={4}
            key={source.name}
          >
            <SourceCard
              source={source}
            />
          </Grid>
        ))}
      </Grid>
    </Box>
  );
}

export default DataSources;