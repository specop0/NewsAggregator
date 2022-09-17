import React from "react";
import { Box, CircularProgress, Grid } from "@mui/material";
import { useGetNews } from "../openapi/backendComponents";
import NewsCard from "./NewsCard";
import { useToolBarDispatch } from "./ToolBarProvider";

const NewsPage: React.FC = () => {
  const [isLatest, setIsLatest] = React.useState<boolean>(false);
  const { data, isLoading, error, refetch, isRefetching } = useGetNews({
    queryParams: {
      isLatest,
    },
  });
  const newsEntries = data ?? [];

  const toolBarDispatch = useToolBarDispatch();

  React.useEffect(() => {
    const onRefresh = async () => {
      if (!isLatest) {
        setIsLatest(true);
      } else {
        refetch();
      }
    };

    toolBarDispatch({
      onRefresh,
    });
  }, [toolBarDispatch, isLatest, refetch]);

  if (isLoading || isRefetching) {
    return (
      <Box
        display="flex"
        minHeight="50vh"
        justifyContent="center"
        alignItems="center"
      >
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return <>Error: {JSON.stringify(error)}</>;
  }

  return (
    <Grid container spacing={2} justifyContent="center">
      {newsEntries.map((news, i) => (
        <Grid item>
          <NewsCard key={i} news={news} />
        </Grid>
      ))}
    </Grid>
  );
};

export default NewsPage;
