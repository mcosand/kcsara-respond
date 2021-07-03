import React from 'react';
import { Link } from 'react-router-dom';
import { observer } from 'mobx-react';
import { Card, CardContent, List, ListItem, ListItemText, Fab, Grid } from '@material-ui/core';
import { Add as AddIcon } from '@material-ui/icons';
import { Activity } from '../store/model/activity';
import { Loadable } from '../store/model/loadable';

export const ActivityList: React.FC<{ activities: Loadable<Activity[]> }> = ({ activities }) => {
  return (
    <Grid container spacing={2} justifyContent="center">
      <Grid item container direction="column" xs={12} sm={10} md={8} lg={6} xl={5} style={{position:'relative'}}>
        <Card style={{ padding: '1em', flexGrow: 1 }}>
          <CardContent style={{flexGrow: 1}}>
            <List>
              {activities.loading ? <div>Loading ...</div> : undefined}
              {activities.obj?.length === 0 ? <ListItem><ListItemText primary="No Recent Activity"/></ListItem> : undefined}
              {activities.obj?.map(a =>
                <ListItem button component="a" key={a.id} href={`/activity/${a.id}`}>
                  <ListItemText primary={(a.number ? `${a.number} `: '') + a.title} />
                </ListItem>
              )}
            </List>
          </CardContent>
        </Card>
        <Fab color="primary" aria-label="add" component={Link} to="/activity/create" style={{position:'absolute', right: '1rem', bottom: '1rem'}}>
          <AddIcon />
        </Fab>
      </Grid>
    </Grid>
  );
}

export default observer(ActivityList);