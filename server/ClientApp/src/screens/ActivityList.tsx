import React from 'react';
import { Link } from 'react-router-dom';
import { observer } from 'mobx-react';
import { List, ListItem, ListItemText, Paper, Fab } from '@material-ui/core';
import { Add as AddIcon } from '@material-ui/icons';
import { Activity } from '../store/model/activity';
import { Loadable } from '../store/model/loadable';

export const ActivityList :React.FC<{activities: Loadable<Activity[]>}> = ({activities}) => {
  return (
    <Paper>
      <List>
        <ListItem><ListItemText primary="above" /></ListItem>
        {activities.loading ? <div>Loading ...</div> : undefined}
        {activities.obj?.map(a => 
          <ListItem button component="a" key={a.id} href={`/activity/${a.id}`}>
            <ListItemText primary={a.title} />
          </ListItem>
          )}
        <ListItem><ListItemText primary="bottom" /></ListItem>
      </List>
      <Fab color="primary" aria-label="add" component={Link} to="/activity/create">
        <AddIcon />
      </Fab>
    </Paper>
  );
}

export default observer(ActivityList);