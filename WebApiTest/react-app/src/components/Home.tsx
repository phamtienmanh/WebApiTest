import React from 'react';

const Post = (props: any) => (
    <div>
        <h3>{props.postdata.title}</h3>
        <h4>{props.postdata.description}</h4>
    </div>
);

function Home() {
    const [message, setMessage] = React.useState("");
    const [posts, setPosts] = React.useState([]);
    const token = localStorage.getItem("token");
    React.useEffect(() => {
        let message = "";
        const fetchData = async () => {
            const resp = await fetch(window.location.origin + "/api/post", {
                method: 'Get',
                headers: {
                    'Accept': 'application/json',
                    'Authorization': 'Bearer ' + token
                }
            });

            if (resp.status === 200) {
                setPosts(await resp.json());
            } else {
                const respData = await resp.json();
                for (const key of Object.keys(respData)) {
                    message += (`${key}: ${respData[key]} `);
                }
                setMessage(message)
            }
        };
        fetchData();
    }, [token]);
    return (
        <div>
            {posts.map((p, index) => <Post key={index} postdata={p}/>)}
            <div>{message}</div>
        </div>
    );
}

export default Home;
