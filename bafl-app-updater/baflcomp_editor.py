"""
BAFL Competition Editor
A Streamlit application to manage CheerComp.json and DrillComp.json files in Azure Blob Storage
"""

import streamlit as st
import json
from datetime import datetime
from azure_blob_service import AzureBlobService
import os
from dotenv import load_dotenv
import extra_streamlit_components as stx

# Load environment variables
load_dotenv()

# Azure configuration from environment variables
SUBSCRIPTION_ID = os.getenv('AZURE_SUBSCRIPTION_ID', '')
RESOURCE_GROUP = os.getenv('AZURE_RESOURCE_GROUP', '')
STORAGE_ACCOUNT = os.getenv('AZURE_STORAGE_ACCOUNT', '')
CONTAINER_NAME = os.getenv('AZURE_CONTAINER_NAME', '')
CHEER_BLOB_NAME = os.getenv('AZURE_CHEER_BLOB_NAME', 'CheerComp.json')
DRILL_BLOB_NAME = os.getenv('AZURE_DRILL_BLOB_NAME', 'DrillComp.json')

# Authentication configuration
APP_USERNAME = os.getenv('APP_USERNAME', 'admin')
APP_PASSWORD = os.getenv('APP_PASSWORD', 'changeme123')

# Initialize cookie manager
cookie_manager = stx.CookieManager()

# Initialize session state
if 'authenticated' not in st.session_state:
    st.session_state.authenticated = False
if 'competition_data' not in st.session_state:
    st.session_state.competition_data = None
if 'blob_service' not in st.session_state:
    st.session_state.blob_service = AzureBlobService(
        STORAGE_ACCOUNT,
        CONTAINER_NAME
    )
if 'has_unsaved_changes' not in st.session_state:
    st.session_state.has_unsaved_changes = False
if 'last_save_time' not in st.session_state:
    st.session_state.last_save_time = None
if 'last_load_time' not in st.session_state:
    st.session_state.last_load_time = None
if 'selected_competition' not in st.session_state:
    # Try to load from cookie, default to 'Cheer'
    saved_competition = cookie_manager.get('selected_competition')
    st.session_state.selected_competition = saved_competition if saved_competition in ['Cheer', 'Drill'] else 'Cheer'
if 'cookie_manager_ready' not in st.session_state:
    st.session_state.cookie_manager_ready = False


def get_current_blob_name():
    """Get the blob name for the currently selected competition"""
    return CHEER_BLOB_NAME if st.session_state.selected_competition == 'Cheer' else DRILL_BLOB_NAME


def get_competition_title():
    """Get the title for the currently selected competition"""
    return f"BAFL {st.session_state.selected_competition} Competition Editor"


def get_competition_icon():
    """Get the icon for the currently selected competition"""
    return "📣" if st.session_state.selected_competition == 'Cheer' else "🎺"


def load_data():
    """Load Competition data from Azure Blob Storage"""
    try:
        blob_name = get_current_blob_name()
        data = st.session_state.blob_service.read_blob(blob_name)
        competition_data = json.loads(data)
        
        # Add unique IDs to schedule items if they don't have them
        for idx, item in enumerate(competition_data.get('Schedule', [])):
            if '_id' not in item:
                item['_id'] = f"item_{idx}_{hash(str(item))}"
        
        st.session_state.competition_data = competition_data
        st.session_state.has_unsaved_changes = False
        st.session_state.last_save_time = None
        st.session_state.last_load_time = datetime.now()
        return True
    except Exception as e:
        st.error(f"❌ Error loading data: {str(e)}")
        return False


def save_data():
    """Save Competition data to Azure Blob Storage"""
    try:
        # Remove internal _id fields before saving
        data_to_save = st.session_state.competition_data.copy()
        for item in data_to_save.get('Schedule', []):
            item.pop('_id', None)
        
        blob_name = get_current_blob_name()
        json_data = json.dumps(data_to_save, indent=2)
        st.session_state.blob_service.write_blob(blob_name, json_data)
        st.session_state.has_unsaved_changes = False
        st.session_state.last_save_time = datetime.now()
        return True
    except Exception as e:
        st.error(f"❌ Error saving data: {str(e)}")
        return False


def mark_unsaved_changes():
    """Mark that there are unsaved changes"""
    st.session_state.has_unsaved_changes = True


def check_authentication():
    """Display login form and check authentication"""
    st.set_page_config(
        page_title="BAFL Competition Editor - Login",
        page_icon="🔐",
        layout="centered"
    )
    
    st.title("🔐 BAFL Competition Editor")
    st.markdown("---")
    
    # Create login form
    with st.form("login_form"):
        st.subheader("Please log in to continue")
        username = st.text_input("Username")
        password = st.text_input("Password", type="password")
        submit = st.form_submit_button("Login", use_container_width=True)
        
        if submit:
            if username == APP_USERNAME and password == APP_PASSWORD:
                st.session_state.authenticated = True
                st.rerun()
            else:
                st.error("❌ Invalid username or password")
    
    st.markdown("---")
    st.caption("🔒 This application is password protected")


def main():
    st.set_page_config(
        page_title="BAFL Competition Editor",
        page_icon="🏆",
        layout="wide",
        menu_items={
            'Get Help': None,
            'Report a bug': None,
            'About': "BAFL Competition Editor - Manage competition files in Azure Blob Storage"
        }
    )
    
    # Dynamic title based on selected competition
    icon = get_competition_icon()
    title = get_competition_title()
    st.title(f"{icon} {title}")
    
    st.markdown("---")
    
    # Control buttons - using sidebar for always-visible access
    with st.sidebar:
        st.header("🏆 Competition")
        
        # Competition selector
        competition_options = ['Cheer', 'Drill']
        selected = st.radio(
            "Select Competition",
            competition_options,
            index=competition_options.index(st.session_state.selected_competition),
            key="competition_selector"
        )
        
        # Handle competition change
        if selected != st.session_state.selected_competition:
            if st.session_state.get('has_unsaved_changes', False):
                st.warning("⚠️ You have unsaved changes! Save them before switching.")
            else:
                st.session_state.selected_competition = selected
                # Save to cookie for next session
                cookie_manager.set('selected_competition', selected, max_age=60*60*24*365)  # 1 year
                st.session_state.competition_data = None
                st.session_state.last_save_time = None
                st.session_state.last_load_time = None
                st.rerun()
        
        st.markdown("---")
        st.header("🔧 Actions")
        
        if st.button("🔄 Load from Azure", use_container_width=True, key="sidebar_load"):
            if load_data():
                st.rerun()
        
        if st.button("💾 Save to Azure", use_container_width=True, key="sidebar_save"):
            if st.session_state.competition_data:
                if save_data():
                    st.rerun()
            else:
                st.warning("⚠️ No data to save. Load data first.")
        
        st.markdown("---")
        
        # Status display
        if st.session_state.last_save_time:
            save_time = st.session_state.last_save_time.strftime('%I:%M:%S %p')
            st.success(f"✅ Saved at {save_time}")
        elif st.session_state.last_load_time:
            load_time = st.session_state.last_load_time.strftime('%I:%M:%S %p')
            st.success(f"✅ Loaded at {load_time}")
        
        # Unsaved changes warning - immediately after status
        if st.session_state.get('has_unsaved_changes', False):
            st.warning("⚠️ Unsaved changes")
        
        # Storage info
        st.markdown("---")
        st.caption("**Azure Storage**")
        st.caption(f"Account: `{STORAGE_ACCOUNT}`")
        st.caption(f"Container: `{CONTAINER_NAME}`")
        st.caption(f"Blob: `{get_current_blob_name()}`")
        
        # Logout button
        st.markdown("---")
        if st.button("🚪 Logout", use_container_width=True, type="secondary"):
            st.session_state.authenticated = False
            st.session_state.competition_data = None
            st.rerun()
    
    # Load data on first run
    if st.session_state.competition_data is None:
        st.info("👆 Click 'Load from Azure' to start editing")
        return
    
    data = st.session_state.competition_data
    
    # Overview Fields Section
    st.header("📋 Event Overview")
    col1, col2 = st.columns(2)
    
    with col1:
        data['Name'] = st.text_input(
            "Event Name",
            value=data.get('Name', ''),
            key="overview_name",
            on_change=mark_unsaved_changes
        )
        data['Date'] = st.text_input(
            "Date (YYYY-MM-DD)",
            value=data.get('Date', ''),
            key="overview_date",
            on_change=mark_unsaved_changes
        )
        data['Message'] = st.text_input(
            "Message",
            value=data.get('Message', ''),
            key="overview_message",
            on_change=mark_unsaved_changes
        )
        data['Information'] = st.text_area(
            "Information",
            value=data.get('Information', ''),
            key="overview_information",
            on_change=mark_unsaved_changes
        )
    
    with col2:
        data['DoorsOpen'] = st.text_input(
            "Doors Open",
            value=data.get('DoorsOpen', ''),
            key="overview_doors_open",
            on_change=mark_unsaved_changes
        )
        data['MoreInfo'] = st.text_input(
            "More Info URL",
            value=data.get('MoreInfo', ''),
            key="overview_more_info",
            on_change=mark_unsaved_changes
        )
        data['Tickets'] = st.text_input(
            "Tickets URL",
            value=data.get('Tickets', ''),
            key="overview_tickets",
            on_change=mark_unsaved_changes
        )
    
    st.session_state.competition_data = data
    
    st.markdown("---")
    
    # Schedule Table Section
    st.header("📅 Schedule")
    
    # Convert schedule to DataFrame for display
    schedule = data.get('Schedule', [])
    
    if len(schedule) == 0:
        st.info("No schedule items yet. Add your first item below.")
    else:
        # Display schedule as editable table
        st.subheader("Current Schedule Items")
        
        for idx, item in enumerate(schedule):
            # Use unique ID for widget keys instead of index
            item_id = item.get('_id', f"item_{idx}")
            
            # Update item with current widget values if they exist in session state
            if f"name_{item_id}" in st.session_state:
                item['Name'] = st.session_state[f"name_{item_id}"]
            if f"start_{item_id}" in st.session_state:
                item['ScheduledStart'] = st.session_state[f"start_{item_id}"]
            
            # Create title without highlight indicator to prevent auto-refresh issues
            expander_title = (
                f"#{idx + 1}: {item.get('Name', 'Unnamed')} - "
                f"{item.get('ScheduledStart', 'No time')}")
            with st.expander(expander_title, expanded=False):
                col1, col2, col3 = st.columns([2, 2, 1])
                
                with col1:
                    new_group = st.text_input(
                        "Group",
                        value=item.get('Group', ''),
                        key=f"group_{item_id}",
                        on_change=mark_unsaved_changes
                    )
                    new_start = st.text_input(
                        "Scheduled Start",
                        value=item.get('ScheduledStart', ''),
                        key=f"start_{item_id}",
                        on_change=mark_unsaved_changes
                    )
                    new_name = st.text_input(
                        "Name",
                        value=item.get('Name', ''),
                        key=f"name_{item_id}",
                        on_change=mark_unsaved_changes
                    )
                    # Update item with new values
                    item['Group'] = new_group
                    item['ScheduledStart'] = new_start
                    item['Name'] = new_name

                with col2:
                    new_status = st.text_input(
                        "Status",
                        value=item.get('Status', ''),
                        key=f"status_{item_id}",
                        on_change=mark_unsaved_changes
                    )
                    new_highlight = st.checkbox(
                        "Highlight",
                        value=item.get('Highlight', False),
                        key=f"highlight_{item_id}",
                        on_change=mark_unsaved_changes
                    )
                    new_notable = st.checkbox(
                        "Notable",
                        value=item.get('Notable', False),
                        key=f"notable_{item_id}",
                        on_change=mark_unsaved_changes
                    )
                    # Update item with new values
                    item['Status'] = new_status
                    item['Highlight'] = new_highlight
                    item['Notable'] = new_notable
                
                with col3:
                    st.write("")
                    st.write("")
                    if st.button("🗑️ Delete", key=f"delete_{idx}", use_container_width=True):
                        schedule.pop(idx)
                        data['Schedule'] = schedule
                        st.session_state.competition_data = data
                        mark_unsaved_changes()
                        st.rerun()
    
    # Add new schedule item
    st.subheader("➕ Add New Schedule Item")
    with st.form("add_schedule_item"):
        col1, col2, col3 = st.columns(3)
        
        with col1:
            new_group = st.text_input("Group (e.g., Cheer, Mascot)")
            new_start = st.text_input("Scheduled Start (e.g., 10:00AM)")
            new_name = st.text_input("Name")
        
        with col2:
            new_status = st.text_input("Status (e.g., Not performed)")
            new_highlight = st.checkbox("Highlight")
            new_notable = st.checkbox("Notable")
        
        with col3:
            st.write("**Position**")
            # Create position options
            position_options = ["End of list"]
            if len(schedule) > 0:
                position_options = [f"Position {i+1}" for i in range(len(schedule))] + ["End of list"]
            
            position_choice = st.selectbox(
                "Insert at",
                position_options,
                index=len(position_options) - 1  # Default to end of list
            )
        
        if st.form_submit_button("➕ Add Item", use_container_width=True):
            import time
            new_item = {
                "_id": f"item_{int(time.time() * 1000)}_{hash(new_name)}",
                "Group": new_group,
                "ScheduledStart": new_start,
                "Name": new_name,
                "Status": new_status,
                "Highlight": new_highlight,
                "Notable": new_notable
            }
            
            # Insert at specified position
            if position_choice == "End of list":
                schedule.append(new_item)
            else:
                # Extract position number from "Position X"
                position = int(position_choice.split()[1]) - 1
                schedule.insert(position, new_item)
            
            data['Schedule'] = schedule
            st.session_state.competition_data = data
            mark_unsaved_changes()
            st.success("✅ Item added (remember to save to Azure)")
            st.rerun()


if __name__ == "__main__":
    # Check authentication first
    if not st.session_state.get('authenticated', False):
        check_authentication()
    else:
        main()
