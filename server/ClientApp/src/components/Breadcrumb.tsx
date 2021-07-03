import { Breadcrumbs, Link, Typography } from '@material-ui/core';
import { Link as RouterLink } from 'react-router-dom';
import { observer } from 'mobx-react';
import * as React from 'react';
import { MainStore } from '../store/main-store';

export const Breadcrumb :React.FC<{store: MainStore}> = ({store}) => (
  <Breadcrumbs aria-label="breadcrumb" style={{margin:'1rem'}}>
    {store.breadcrumbs.map(bc => (
      bc.isLast ? <Typography color="text.primary" key={bc.to}>{bc.name}</Typography>
                : <Link key={bc.to} underline="hover" color="inherit" to={bc.to} component={RouterLink}>{bc.name}</Link> 
    ))}
  </Breadcrumbs>
)

export default observer(Breadcrumb);