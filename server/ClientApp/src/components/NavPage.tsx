import * as React from 'react';
import { Box, Grid } from '@material-ui/core';
import Breadcrumb from './Breadcrumb';
import { MainStore } from '../store/main-store';
import BottomBar from './BottomBar';
import { Page } from './Page';

export const NavPage: React.FC<{ store: MainStore }> = ({ store, children }) => {
  return (
    <Page>
      <Grid container direction="column" style={{flexWrap:'initial'}} className="scrolling">
        <Breadcrumb store={store} />
        <Box style={{ display: 'flex', flexDirection: 'column', flexGrow: 1, position: 'relative' }}>
          {children}
        </Box>
      </Grid>
      <BottomBar store={store} />
    </Page>
  );
}