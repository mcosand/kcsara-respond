import * as React from 'react';
import { Grid, Paper } from '@material-ui/core';
import Breadcrumb from './Breadcrumb';
import { MainStore } from '../store/main-store';

export const Page :React.FC<{ store: MainStore }> = ({store, children}) => {
  return (
    <Grid container spacing={2} justifyContent="center">
      <Grid item container direction="column" xs={12} sm={10} md={8} lg={6} xl={5}>
        <Paper style={{ flexGrow: 1, display: 'flex', flexDirection:'column' }}>
          <Breadcrumb store={store} />
          {children}
        </Paper>
      </Grid>
    </Grid>
  );
}