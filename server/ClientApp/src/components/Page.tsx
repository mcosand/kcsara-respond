import * as React from 'react';
import { Box, Grid, Paper } from '@material-ui/core';
import Breadcrumb from './Breadcrumb';
import { MainStore } from '../store/main-store';
import BottomBar from './BottomBar';

export const Page :React.FC<{ store: MainStore }> = ({store, children}) => {
  return (
    <Grid container spacing={2} justifyContent="center">
      <Grid item container direction="column" xs={12} sm={10} md={8} lg={6} xl={5}>
        <Paper style={{ flexGrow: 1, display: 'flex', flexDirection:'column' }}>
          <Breadcrumb store={store} />
          <Box style={{display: 'flex', flexDirection:'column', flexGrow: 1, position: 'relative' }}>
            {children}
          </Box>
          <BottomBar store={store} />
        </Paper>
      </Grid>
    </Grid>
  );
}