import {
  Dashboard,
  Storage,
  Dataset,
  AutoAwesome,
  AccountTree,
} from "@mui/icons-material";
import {
  Box,
  Divider,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Typography,
} from "@mui/material";
import { useLocation, useNavigate } from "react-router-dom";

const menuItems = [
  {
    title: "Dashboard",
    path: "/dashboard",
    icon: <Dashboard />,
  },
  {
    title: "Data Sources",
    path: "/data-sources",
    icon: <Storage />,
  },
  {
    title: "Collected Data",
    path: "/collected-data",
    icon: <Dataset />,
  },
  {
    title: "Knowledge Extraction",
    path: "/knowledge-extraction",
    icon: <AutoAwesome />,
  },
  {
    title: "Knowledge Graph",
    path: "/knowledge-graph",
    icon: <AccountTree />,
  },
];

function Sidebar() {
  const navigate = useNavigate();
  const location = useLocation();

  return (
    <Box
      sx={{
        width: 260,
        minHeight: "100vh",
        backgroundColor: "#111827",
        color: "white",
        display: "flex",
        flexDirection: "column",
      }}
    >
      <Box sx={{ px: 3, py: 3 }}>
        <Typography variant="h5" fontWeight="bold">
          BASEERA
        </Typography>

        <Typography
          variant="body2"
          sx={{ color: "#9CA3AF", mt: 0.5 }}
        >
          Social Intelligence
        </Typography>
      </Box>

      <Divider sx={{ borderColor: "#374151" }} />

      <List sx={{ px: 1.5, py: 2 }}>
        {menuItems.map((item) => {
          const active = location.pathname === item.path;

          return (
            <ListItemButton
              key={item.path}
              selected={active}
              onClick={() => navigate(item.path)}
              sx={{
                borderRadius: 2,
                mb: 0.5,
                color: "#D1D5DB",

                "& .MuiListItemIcon-root": {
                  color: "#9CA3AF",
                },

                "&.Mui-selected": {
                  backgroundColor: "#1F2937",
                  color: "white",
                },

                "&.Mui-selected .MuiListItemIcon-root": {
                  color: "#A78BFA",
                },

                "&:hover": {
                  backgroundColor: "#1F2937",
                },
              }}
            >
              <ListItemIcon>{item.icon}</ListItemIcon>

              <ListItemText primary={item.title} />
            </ListItemButton>
          );
        })}
      </List>
    </Box>
  );
}

export default Sidebar;