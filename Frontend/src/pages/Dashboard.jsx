import {
  AccountTree,
  AutoAwesome,
  Description,
  Storage,
} from "@mui/icons-material";
import {
  Box,
  Card,
  CardContent,
  Chip,
  Grid,
  Stack,
  Typography,
} from "@mui/material";

const stats = [
  {
    title: "Data Sources",
    value: "3",
    subtitle: "Connected platforms",
    icon: <Storage />,
  },
  {
    title: "Documents",
    value: "124",
    subtitle: "Collected documents",
    icon: <Description />,
  },
  {
    title: "Entities",
    value: "87",
    subtitle: "Extracted entities",
    icon: <AutoAwesome />,
  },
  {
    title: "Relationships",
    value: "53",
    subtitle: "Knowledge graph relations",
    icon: <AccountTree />,
  },
];

function Dashboard() {
  return (
    <Box>
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" fontWeight={700}>
          Dashboard
        </Typography>

        <Typography
          variant="body1"
          color="text.secondary"
          sx={{ mt: 1 }}
        >
          Monitor your data collection and knowledge extraction pipeline.
        </Typography>
      </Box>

      <Grid container spacing={3}>
        {stats.map((stat) => (
          <Grid item xs={12} sm={6} lg={3} key={stat.title}>
            <Card
              elevation={0}
              sx={{
                border: "1px solid #E5E7EB",
                borderRadius: 3,
              }}
            >
              <CardContent>
                <Stack
                  direction="row"
                  justifyContent="space-between"
                  alignItems="flex-start"
                >
                  <Box>
                    <Typography
                      variant="body2"
                      color="text.secondary"
                    >
                      {stat.title}
                    </Typography>

                    <Typography
                      variant="h4"
                      fontWeight={700}
                      sx={{ mt: 1 }}
                    >
                      {stat.value}
                    </Typography>

                    <Typography
                      variant="caption"
                      color="text.secondary"
                    >
                      {stat.subtitle}
                    </Typography>
                  </Box>

                  <Box
                    sx={{
                      width: 44,
                      height: 44,
                      borderRadius: 2,
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "center",
                      backgroundColor: "#F3E8FF",
                      color: "#7C3AED",
                    }}
                  >
                    {stat.icon}
                  </Box>
                </Stack>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      <Card
        elevation={0}
        sx={{
          mt: 4,
          border: "1px solid #E5E7EB",
          borderRadius: 3,
        }}
      >
        <CardContent sx={{ p: 3 }}>
          <Typography variant="h6" fontWeight={700}>
            Knowledge Pipeline
          </Typography>

          <Typography
            variant="body2"
            color="text.secondary"
            sx={{ mt: 0.5, mb: 3 }}
          >
            From social media data to structured knowledge.
          </Typography>

          <Stack
            direction={{ xs: "column", md: "row" }}
            spacing={2}
            alignItems="center"
          >
            <PipelineStep
              number="01"
              title="Data Collection"
              description="Collect data from connected platforms."
              status="Active"
            />

            <Typography sx={{ display: { xs: "none", md: "block" } }}>
              →
            </Typography>

            <PipelineStep
              number="02"
              title="Knowledge Extraction"
              description="Extract entities and relationships using AI."
              status="Ready"
            />

            <Typography sx={{ display: { xs: "none", md: "block" } }}>
              →
            </Typography>

            <PipelineStep
              number="03"
              title="Knowledge Graph"
              description="Store and explore connected knowledge."
              status="Ready"
            />
          </Stack>
        </CardContent>
      </Card>
    </Box>
  );
}

function PipelineStep({ number, title, description, status }) {
  return (
    <Box
      sx={{
        flex: 1,
        width: "100%",
        p: 2.5,
        borderRadius: 2,
        backgroundColor: "#F8FAFC",
        border: "1px solid #E5E7EB",
      }}
    >
      <Typography
        variant="caption"
        fontWeight={700}
        color="primary"
      >
        {number}
      </Typography>

      <Typography variant="h6" fontWeight={600} sx={{ mt: 1 }}>
        {title}
      </Typography>

      <Typography
        variant="body2"
        color="text.secondary"
        sx={{ mt: 0.5, minHeight: 40 }}
      >
        {description}
      </Typography>

      <Chip
        label={status}
        size="small"
        sx={{ mt: 2 }}
      />
    </Box>
  );
}

export default Dashboard;