import React from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { observer } from 'mobx-react';
import { Card, CardContent, List, ListItem, ListItemText, Fab, Grid } from '@material-ui/core';
import { Add as AddIcon } from '@material-ui/icons';
import { MainStore } from '../../store/main-store';
import { Page } from '../../components/Page';

export const ActivityList: React.FC<{ store: MainStore }> = ({ store }) => {
  const activities = store.activities
  return (
    <Page store={store}>
      <Card style={{ flexGrow: 1 }}>
        <CardContent style={{flexGrow: 1}}>
          <List>
            {activities.loading ? <div>Loading ...</div> : undefined}
            {activities.obj?.length === 0 ? <ListItem><ListItemText primary="No Recent Activity"/></ListItem> : undefined}
            {activities.obj?.map(a =>
              <ListItem button component={RouterLink} key={a.id} to={`/activity/${a.id}`}>
                <ListItemText primary={(a.number ? `${a.number} `: '') + a.title} />
              </ListItem>
            )}
          </List>
        </CardContent>
      </Card>
      <Fab color="primary" aria-label="add" component={RouterLink} to="/activity/create" style={{position:'absolute', right: '1rem', bottom: '1rem'}}>
        <AddIcon />
      </Fab>
    </Page>
  );
}

export default observer(ActivityList);