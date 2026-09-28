import {
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  Grid,
  Stack,
  Typography,
} from "@mui/material";

import HubIcon from "@mui/icons-material/Hub";
import FacebookIcon from "@mui/icons-material/Facebook";
import InstagramIcon from "@mui/icons-material/Instagram";
import RedditIcon from "@mui/icons-material/Language";
// import CloudIcon from "@mui/icons-material/Cloud";
import StorageIcon from "@mui/icons-material/Storage";
import LinkIcon from "@mui/icons-material/Link";

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

const otherSources = [
  {
    name: "Reddit",
    description:
      "Collect public discussions and community content.",
    icon: <RedditIcon sx={{ fontSize: 30 }} />,
  },
  {
    name: "Bluesky",
    description:
      "Collect social content and conversations from Bluesky.",
    icon: <LinkIcon sx={{ fontSize: 30 }} />,
  },
  {
    name: "Mastodon",
    description:
      "Collect social content from connected Mastodon sources.",
    icon: <StorageIcon sx={{ fontSize: 30 }} />,
  },
];

function SourceCard({ source }) {
  return (
    <Card
      sx={{
        height: "100%",
        borderRadius: 3,
        border: "1px solid",
        borderColor: "divider",
        boxShadow: "none",
        transition: "all 0.2s ease",
        "&:hover": {
          transform: "translateY(-3px)",
          boxShadow: "0 8px 24px rgba(0,0,0,0.08)",
          borderColor: "primary.light",
        },
      }}
    >
      <CardContent sx={{ p: 3 }}>
        <Stack
          direction="row"
          justifyContent="space-between"
          alignItems="flex-start"
        >
          <Stack direction="row" spacing={2} alignItems="center">
            <Box
              sx={{
                width: 56,
                height: 56,
                borderRadius: 2,
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                bgcolor: "action.hover",
                color: "text.primary",
              }}
            >
              {source.icon}
            </Box>

            <Box>
              <Typography variant="h6" fontWeight={700}>
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
            label="Not Connected"
            size="small"
            variant="outlined"
            sx={{
              color: "text.secondary",
              borderColor: "divider",
            }}
          />
        </Stack>

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
      </CardContent>
    </Card>
  );
}

function DataSources() {
  return (
    <Box>
      {/* Page Header */}
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" fontWeight={700} gutterBottom>
          Data Sources
        </Typography>

        <Typography color="text.secondary">
          Connect your social platforms and collect data for the Baseera
          knowledge pipeline.
        </Typography>
      </Box>

      {/* Meta Authentication */}
      <Card
        sx={{
          mb: 5,
          borderRadius: 3,
          border: "1px solid",
          borderColor: "primary.main",
          boxShadow: "none",
          background:
            "linear-gradient(135deg, rgba(25,118,210,0.06), rgba(255,255,255,1))",
        }}
      >
        <CardContent sx={{ p: { xs: 3, md: 4 } }}>
          <Stack
            direction={{ xs: "column", md: "row" }}
            justifyContent="space-between"
            alignItems={{ xs: "flex-start", md: "center" }}
            spacing={3}
          >
            <Stack direction="row" spacing={2} alignItems="center">
              <Box
                sx={{
                  width: 64,
                  height: 64,
                  borderRadius: 2.5,
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  bgcolor: "primary.main",
                  color: "white",
                }}
              >
                <HubIcon sx={{ fontSize: 34 }} />
              </Box>

              <Box>
                <Stack
                  direction="row"
                  spacing={1}
                  alignItems="center"
                  flexWrap="wrap"
                >
                  <Typography variant="h6" fontWeight={700}>
                    Meta Account
                  </Typography>

                  <Chip
                    label="Authentication"
                    size="small"
                    color="primary"
                  />
                </Stack>

                <Typography
                  color="text.secondary"
                  sx={{ mt: 0.5 }}
                >
                  Facebook Pages & Instagram Business
                </Typography>
              </Box>
            </Stack>

            <Button
              variant="contained"
              size="small"
              startIcon={<LinkIcon />}
              sx={{
                borderRadius: 2,
                textTransform: "none",
                fontWeight: 600,
                px: 3,
              }}
            >
              Connect Meta
            </Button>
          </Stack>

          <Box
            sx={{
              mt: 3,
              pt: 3,
              borderTop: "1px solid",
              borderColor: "divider",
            }}
          >
            <Typography color="text.secondary" lineHeight={1.7}>
              Connect your Meta business account once. Baseera will then
              discover your managed Facebook Pages and connected Instagram
              Business accounts.
            </Typography>
          </Box>
        </CardContent>
      </Card>

      {/* Connected Social Sources */}
      <Box sx={{ mb: 2 }}>
        <Typography variant="h6" fontWeight={700}>
          Social Sources
        </Typography>

        <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
          Each platform is handled by its own data connector.
        </Typography>
      </Box>

      <Grid container spacing={3} sx={{ mb: 5 }}>
        {socialSources.map((source) => (
          <Grid item xs={12} md={6} key={source.name}>
            <SourceCard source={source} />
          </Grid>
        ))}
      </Grid>

      {/* Other Sources */}
      <Box sx={{ mb: 2 }}>
        <Typography variant="h6" fontWeight={700}>
          Other Sources
        </Typography>

        <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
          Additional social platforms supported by Baseera.
        </Typography>
      </Box>

      <Grid container spacing={3}>
        {otherSources.map((source) => (
          <Grid item xs={12} md={6} lg={4} key={source.name}>
            <SourceCard source={source} />
          </Grid>
        ))}
      </Grid>
    </Box>
  );
}

export default DataSources;