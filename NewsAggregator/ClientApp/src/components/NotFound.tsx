import React from "react";
import { useLocation } from "react-router-dom";
import { Typography } from "@mui/material";

const NotFound: React.FC = () => {
  const location = useLocation();
  const path = location.pathname ?? "/";
  return <Typography><code>{path}</code> not found</Typography>;
};

export default NotFound;
