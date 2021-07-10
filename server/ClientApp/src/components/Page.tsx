import * as React from 'react';
import { Grid, Paper } from '@material-ui/core';

export const Page :React.FC = ({children}) => {
  return (
    <Grid container justifyContent="center" direction="row" sx={{ flexGrow: 1 }}>
      <Grid item container direction="column" xs={12} sm={10} md={8} lg={6} xl={5}>
        <Paper id="page" >
          {children}
        </Paper>
      </Grid>
    </Grid>
  );
}