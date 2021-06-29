import React from 'react';
import { List, ListItem, ListItemText } from '@material-ui/core';
import { observer } from 'mobx-react';
import { Activity } from '../store/model/activity';

export const ActivityList :React.FC<{activities: Activity[]}> = ({activities}) => {
  return (
    <List>
      <ListItem><ListItemText primary="above" /></ListItem>
      {activities.map(a => 
        <ListItem button component="a" key={a.id} href={`/activity/${a.id}`}>
          <ListItemText primary={a.title} />
        </ListItem>
        )}
      <ListItem><ListItemText primary="bottom" /></ListItem>
    </List>
  );
}

export default observer(ActivityList)