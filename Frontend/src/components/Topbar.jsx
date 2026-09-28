import {
  AppBar,
  Avatar,
  Box,
  Toolbar,
  Typography,
} from "@mui/material";

function Topbar() {
  return (
    <AppBar
      position="static"
      elevation={0}
      sx={{
        backgroundColor: "white",
        color: "#111827",
        borderBottom: "1px solid #E5E7EB",
      }}
    >
      <Toolbar sx={{ justifyContent: "space-between" }}>
        <Box>
          <Typography variant="h6" fontWeight="600">
            Baseera Dashboard
          </Typography>

          <Typography variant="body2" color="text.secondary">
            Turn social data into actionable knowledge
          </Typography>
        </Box>

        <Avatar sx={{ backgroundColor: "#7C3AED" }}>B</Avatar>
      </Toolbar>
    </AppBar>
  );
}

export default Topbar;